using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BurLunch.AuthAPI.Data;
using BurLunch.AuthAPI.Models;

[Route("api/[controller]")]
[ApiController]
public class ScheduleController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ScheduleController(ApplicationDbContext context)
    {
        _context = context;
    }

    // Получить список расписаний
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

    // Получить расписание по ID
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

    // Создать расписание
    [HttpPost]
    public async Task<IActionResult> CreateSchedule([FromBody] Schedule schedule)
    {
        if (schedule == null || schedule.WeeklyMenuId <= 0)
            return BadRequest("Некорректные данные для создания расписания");

        _context.Schedules.Add(schedule);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetScheduleById), new { id = schedule.Id }, schedule);
    }

    //[HttpPost("BulkCreate")]
    //public IActionResult BulkCreateSchedules([FromBody] List<CreateScheduleRequest> schedules)
    //{
    //    if (schedules == null || schedules.Count == 0)
    //        return BadRequest("Данные для создания расписания отсутствуют.");

    //    foreach (var scheduleData in schedules)
    //    {
    //        var schedule = new Schedule
    //        {
    //            Date =  scheduleData.Date,
    //            WeeklyMenuId = scheduleData.WeeklyMenuId
    //        };

    //        _context.Schedules.Add(schedule);
    //    }

    //    _context.SaveChanges();

    //    return Ok(new { Message = "Расписания успешно созданы." });
    //}

    [HttpPost("BulkCreate")]
    public IActionResult BulkCreateSchedules([FromBody] List<CreateScheduleRequest> schedules)
    {
        if (schedules == null || schedules.Count == 0)
            return BadRequest("Данные для создания расписания отсутствуют.");

        foreach (var scheduleData in schedules)
        {
            // Преобразуем дату в UTC
            var utcDate = scheduleData.Date.ToUniversalTime();

            var schedule = new Schedule
            {
                Date = utcDate, // Сохраняем дату в формате UTC
                WeeklyMenuId = scheduleData.WeeklyMenuId
            };

            _context.Schedules.Add(schedule);
        }

        _context.SaveChanges();

        return Ok(new { Message = "Расписания успешно созданы." });
    }


    // Обновить расписание
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

    // Удалить расписание
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
}
