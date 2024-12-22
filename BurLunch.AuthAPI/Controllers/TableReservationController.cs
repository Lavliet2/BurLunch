using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BurLunch.AuthAPI.Data;
using BurLunch.AuthAPI.Models;
using System.Text.Json;

[Route("api/[controller]")]
[ApiController]
public class TableReservationController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public TableReservationController(ApplicationDbContext context)
    {
        _context = context;
    }

    //// Получить список бронирований
    //[HttpGet]
    //public async Task<IActionResult> GetReservations()
    //{
    //    var reservations = await _context.TableReservations
    //        .Include(tr => tr.Table)
    //        .Include(tr => tr.Schedule)
    //        .ThenInclude(s => s.WeeklyMenu)
    //        .Include(tr => tr.User)
    //        .ToListAsync();

    //    return Ok(reservations);
    //}
    [HttpGet]
    public async Task<IActionResult> GetReservations()
    {
        var reservations = await _context.TableReservations
            .Include(tr => tr.Table)
            .Include(tr => tr.Schedule)
            .Include(tr => tr.User)
            .Include(tr => tr.SelectedDishes) // Загружаем связанные блюда
            .Select(r => new
            {
                r.Id,
                r.ScheduleId,
                r.TableId,
                Table = new
                {
                    r.Table.Id,
                    r.Table.Seats,
                    r.Table.Description
                },
                r.UserId,
                r.User.Username,
                r.SeatsReserved,
                r.ReservationTime,
                SelectedDishes = r.SelectedDishes.Select(d => new
                {
                    d.Id,
                    d.Name,
                    d.Description
                })
            })
            .ToListAsync();

        return Ok(reservations);
    }

    //[HttpGet("reservations/{scheduleId}")]
    //public async Task<IActionResult> GetReservations(int scheduleId)
    //{
    //    var reservations = await _context.TableReservations
    //        .Include(tr => tr.Table)
    //        .Include(tr => tr.User)
    //        .Where(tr => tr.ScheduleId == scheduleId)
    //        .ToListAsync();

    //    var result = reservations.Select(r => new
    //    {
    //        TableId = r.TableId,
    //        UserName = r.User.Username,
    //        SeatsReserved = r.SeatsReserved,
    //        ReservationTime = r.ReservationTime
    //    });

    //    return Ok(result);
    //}
    [HttpGet("reservations/{scheduleId}")]
    public async Task<IActionResult> GetReservations(int scheduleId)
    {
        var reservations = await _context.TableReservations
            .Include(tr => tr.Table)
            .Include(tr => tr.User)
            .Include(tr => tr.SelectedDishes) // Включаем связанные блюда
            .Where(tr => tr.ScheduleId == scheduleId)
            .Select(r => new
            {
                r.Id,
                r.ScheduleId,
                r.TableId,
                Table = new
                {
                    r.Table.Id,
                    r.Table.Seats,
                    r.Table.Description
                },
                r.UserId,
                r.SeatsReserved,
                r.ReservationTime,
                SelectedDishes = r.SelectedDishes.Select(d => new
                {
                    d.Id,
                    d.Name,
                    d.Description
                })
            })
            .ToListAsync();

        return Ok(reservations);
    }


    [HttpPost("book")]
    public async Task<IActionResult> BookTable([FromBody] JsonElement reservationData)
    {
        var userId = reservationData.GetProperty("UserId").GetInt32();
        var tableId = reservationData.GetProperty("TableId").GetInt32();
        var scheduleId = reservationData.GetProperty("ScheduleId").GetInt32();
        var seatsReserved = reservationData.GetProperty("SeatsReserved").GetInt32();
        var dishIds = reservationData.GetProperty("DishIds").EnumerateArray().Select(d => d.GetInt32()).ToList();

        var table = await _context.Tables.FindAsync(tableId);
        if (table == null) return NotFound("Стол не найден.");

        var schedule = await _context.Schedules.FindAsync(scheduleId);
        if (schedule == null) return NotFound("Расписание не найдено.");

        var reservedSeats = await _context.TableReservations
            .Where(tr => tr.TableId == tableId && tr.ScheduleId == scheduleId)
            .SumAsync(tr => tr.SeatsReserved);

        if (reservedSeats + seatsReserved > table.Seats)
            return BadRequest("Недостаточно свободных мест за столом.");

        var dishes = await _context.Dishes
            .Where(d => dishIds.Contains(d.Id))
            .ToListAsync();

        if (dishes.Count != dishIds.Count)
        {
            return BadRequest("Некоторые выбранные блюда не найдены.");
        }

        var reservation = new TableReservation
        {
            TableId = tableId,
            ScheduleId = scheduleId,
            UserId = userId,
            SeatsReserved = seatsReserved,
            ReservationTime = DateTime.UtcNow,
            SelectedDishes = dishes // Сохраняем выбранные блюда
        };

        _context.TableReservations.Add(reservation);
        await _context.SaveChangesAsync();

        return Ok(new { Message = "Выбор успешно сохранён!" });
    }




    // Удалить бронирование
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteReservation(int id)
    {
        var reservation = await _context.TableReservations.FindAsync(id);
        if (reservation == null)
            return NotFound("Бронирование не найдено");

        _context.TableReservations.Remove(reservation);
        await _context.SaveChangesAsync();

        return Ok(new { Message = "Бронирование успешно удалено" });
    }
}
