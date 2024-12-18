using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BurLunch.AuthAPI.Data;
using BurLunch.AuthAPI.Models;

[Route("api/[controller]")]
[ApiController]
public class TableReservationController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public TableReservationController(ApplicationDbContext context)
    {
        _context = context;
    }

    // Получить список бронирований
    [HttpGet]
    public async Task<IActionResult> GetReservations()
    {
        var reservations = await _context.TableReservations
            .Include(tr => tr.Table)
            .Include(tr => tr.Schedule)
            .ThenInclude(s => s.WeeklyMenu)
            .Include(tr => tr.User)
            .ToListAsync();

        return Ok(reservations);
    }

    // Забронировать стол
    [HttpPost("book")]
    public async Task<IActionResult> BookTable([FromBody] TableReservation reservation)
    {
        var table = await _context.Tables.FindAsync(reservation.TableId);
        if (table == null)
            return NotFound("Стол не найден");

        var schedule = await _context.Schedules.FindAsync(reservation.ScheduleId);
        if (schedule == null)
            return NotFound("Расписание не найдено");

        var reservedSeats = _context.TableReservations
            .Where(tr => tr.TableId == reservation.TableId && tr.ScheduleId == reservation.ScheduleId)
            .Sum(tr => tr.SeatsReserved);

        if (reservedSeats + reservation.SeatsReserved > table.Seats)
            return BadRequest("Недостаточно свободных мест за столом");

        reservation.ReservationTime = DateTime.UtcNow;
        _context.TableReservations.Add(reservation);
        await _context.SaveChangesAsync();

        return Ok(new { Message = "Стол успешно забронирован" });
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
