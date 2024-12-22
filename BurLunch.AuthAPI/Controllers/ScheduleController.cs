using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BurLunch.AuthAPI.Data;
using BurLunch.AuthAPI.Models;
using BurLunch.AuthAPI.Utils;
using System.Text.Json;

[Route("api/[controller]")]
[ApiController]
public class ScheduleController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ScheduleController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetSchedules()
    {
        var schedules = await _context.Schedules
            .Include(s => s.WeeklyMenu)
            .Include(s => s.TableReservations)
            .ThenInclude(tr => tr.Table)
            .ToListAsync();

        return Ok(schedules);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetScheduleById(int id)
    {
        var schedule = await _context.Schedules
            .Include(s => s.WeeklyMenu)
            .Include(s => s.TableReservations)
            .ThenInclude(tr => tr.Table)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (schedule == null)
            return NotFound("Расписание не найдено");

        return Ok(schedule);
    }

    [HttpGet("ByDate")]
    public async Task<IActionResult> GetScheduleByDate([FromQuery] string date)
    {
        try
        {
            var parsedDate = JsonSerializer.Deserialize<DateTime>(
                $"\"{date}\"",
                new JsonSerializerOptions
                {
                    Converters = { new FlexibleDateTimeConverter() }
                }
            );

            var targetDate = parsedDate.Date; 
            var schedule = await _context.Schedules
                .Include(s => s.WeeklyMenu) 
                .FirstOrDefaultAsync(s => s.Date.Date == targetDate);

            if (schedule == null)
                return NotFound(new { Message = $"Расписание на {targetDate:dd.MM.yyyy} не создано." });

            return Ok(schedule);
        }
        catch (JsonException ex)
        {
            return BadRequest(new { Message = $"Неверный формат даты: {date}. Ошибка: {ex.Message}" });
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateSchedule([FromBody] Schedule schedule)
    {
        if (schedule == null || schedule.WeeklyMenuId <= 0)
            return BadRequest("Некорректные данные для создания расписания");

        _context.Schedules.Add(schedule);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetScheduleById), new { id = schedule.Id }, schedule);
    }

    [HttpPost("BulkCreate")]
    public IActionResult BulkCreateSchedules([FromBody] List<CreateScheduleRequest> schedules)
    {
        if (schedules == null || schedules.Count == 0)
            return BadRequest("Данные для создания расписания отсутствуют.");

        foreach (var scheduleData in schedules)
        {
            var utcDate = scheduleData.Date.ToUniversalTime();
            var schedule = new Schedule
            {
                Date = utcDate, 
                WeeklyMenuId = scheduleData.WeeklyMenuId
            };

            _context.Schedules.Add(schedule);
        }

        _context.SaveChanges();

        return Ok(new { Message = "Расписания успешно созданы." });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateSchedule(int id, [FromBody] Schedule updatedSchedule)
    {
        var schedule = await _context.Schedules.FindAsync(id);
        if (schedule == null)
            return NotFound("Расписание не найдено");

        schedule.Date = updatedSchedule.Date;
        schedule.WeeklyMenuId = updatedSchedule.WeeklyMenuId;

        await _context.SaveChangesAsync();

        return Ok(new { Message = "Расписание успешно обновлено" });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteSchedule(int id)
    {
        var schedule = await _context.Schedules.FindAsync(id);
        if (schedule == null)
            return NotFound("Расписание не найдено");

        _context.Schedules.Remove(schedule);
        await _context.SaveChangesAsync();

        return Ok(new { Message = "Расписание успешно удалено" });
    }

    [HttpPost("BulkDelete")]
    public IActionResult BulkDeleteSchedules([FromBody] List<DateTime> dates)
    {
        if (dates == null || dates.Count == 0)
            return BadRequest("Даты для удаления отсутствуют.");

        foreach (var date in dates)
        {
            var schedules = _context.Schedules.Where(s => s.Date.Date == date.Date).ToList();

            if (schedules.Any())
            {
                _context.Schedules.RemoveRange(schedules);
            }
        }

        _context.SaveChanges();

        return Ok(new { Message = "Расписания успешно удалены." });
    }
}
