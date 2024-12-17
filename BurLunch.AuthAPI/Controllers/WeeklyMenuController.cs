using BurLunch.AuthAPI.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using BurLunch.AuthAPI.Models;

[Route("api/[controller]")]
[ApiController]
public class WeeklyMenuController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public WeeklyMenuController(ApplicationDbContext context)
    {
        _context = context;
    }

    // Получить все карточки меню на неделю
    [HttpGet]
    public IActionResult GetWeeklyMenus()
    {
        var menus = _context.WeeklyMenuCards
            .Include(w => w.DailyMenus)
                .ThenInclude(d => d.Dishes)
                    .ThenInclude(d => d.DishType)
            .ToList();

        return Ok(menus);
    }

    // Добавить карточку меню на неделю
    [HttpPost]
    public IActionResult AddWeeklyMenu([FromBody] WeeklyMenuCard weeklyMenu)
    {
        if (weeklyMenu == null || weeklyMenu.DailyMenus == null || !weeklyMenu.DailyMenus.Any())
        {
            return BadRequest("Карточка меню на неделю не может быть пустой.");
        }

        foreach (var dailyMenu in weeklyMenu.DailyMenus)
        {
            foreach (var dish in dailyMenu.Dishes)
            {
                dish.DishType = null; // Убедитесь, что связь DishType указывается через Id
            }
        }

        _context.WeeklyMenuCards.Add(weeklyMenu);
        _context.SaveChanges();

        return CreatedAtAction(nameof(GetWeeklyMenus), new { id = weeklyMenu.Id }, weeklyMenu);
    }

    // Удалить карточку меню на неделю
    [HttpDelete("{id}")]
    public IActionResult DeleteWeeklyMenu(int id)
    {
        var weeklyMenu = _context.WeeklyMenuCards
            .Include(w => w.DailyMenus)
            .ThenInclude(d => d.Dishes)
            .FirstOrDefault(w => w.Id == id);

        if (weeklyMenu == null)
        {
            return NotFound("Карточка меню не найдена.");
        }

        _context.WeeklyMenuCards.Remove(weeklyMenu);
        _context.SaveChanges();

        return Ok(new { Message = "Карточка меню успешно удалена." });
    }

    // Добавить блюдо в дневное меню
    [HttpPost("{weeklyMenuId}/daily/{dayOfWeek}/add-dish")]
    public IActionResult AddDishToDailyMenu(int weeklyMenuId, DayOfWeek dayOfWeek, [FromBody] Dish dish)
    {
        var weeklyMenu = _context.WeeklyMenuCards
            .Include(w => w.DailyMenus)
            .FirstOrDefault(w => w.Id == weeklyMenuId);

        if (weeklyMenu == null)
        {
            return NotFound("Карточка меню на неделю не найдена.");
        }

        var dailyMenu = weeklyMenu.DailyMenus.FirstOrDefault(dm => dm.DayOfWeek == dayOfWeek);
        if (dailyMenu == null)
        {
            dailyMenu = new DailyMenu { DayOfWeek = dayOfWeek, WeeklyMenuCardId = weeklyMenuId };
            weeklyMenu.DailyMenus.Add(dailyMenu);
        }

        dish.DishType = null; // Убедитесь, что связь DishType указывается через Id
        dailyMenu.Dishes.Add(dish);
        _context.SaveChanges();

        return Ok(dailyMenu);
    }

    // Удалить дневное меню
    [HttpDelete("daily/{id}")]
    public IActionResult DeleteDailyMenu(int id)
    {
        var dailyMenu = _context.DailyMenus
            .Include(d => d.Dishes)
            .FirstOrDefault(d => d.Id == id);

        if (dailyMenu == null)
        {
            return NotFound("Дневное меню не найдено.");
        }

        _context.DailyMenus.Remove(dailyMenu);
        _context.SaveChanges();

        return Ok(new { Message = "Дневное меню успешно удалено." });
    }

    // Удалить блюдо
    [HttpDelete("dish/{id}")]
    public IActionResult DeleteDish(int id)
    {
        var dish = _context.Dishes.FirstOrDefault(d => d.Id == id);

        if (dish == null)
        {
            return NotFound("Блюдо не найдено.");
        }

        _context.Dishes.Remove(dish);
        _context.SaveChanges();

        return Ok(new { Message = "Блюдо успешно удалено." });
    }
}
