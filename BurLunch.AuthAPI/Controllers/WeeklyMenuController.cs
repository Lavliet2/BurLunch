using BurLunch.AuthAPI.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

    // Получить все карточки недельного меню
    //[HttpGet]
    //public IActionResult GetWeeklyMenus()
    //{
    //    var weeklyMenus = _context.WeeklyMenuCards
    //        .Include(w => w.Dishes) // Подгружаем список блюд
    //        .ThenInclude(d => d.DishType) // Подгружаем типы блюд
    //        .ToList();

    //    return Ok(weeklyMenus);
    //}
    [HttpGet]
    public IActionResult GetWeeklyMenus()
    {
        var menus = _context.WeeklyMenuCards
            .Include(w => w.Dishes)
            .ThenInclude(d => d.DishType)
            .Select(w => new
            {
                w.Id,
                w.Name,
                Dishes = w.Dishes.Select(d => new
                {
                    d.Id,
                    d.Name,
                    d.Description,
                    DishType = d.DishType.Name
                })
            })
            .ToList();

        return Ok(menus);
    }

    [HttpGet("{weeklyMenuId}/available-dishes")]
    public IActionResult GetAvailableDishes(int weeklyMenuId)
    {
        var weeklyMenu = _context.WeeklyMenuCards
            .Include(w => w.Dishes)
            .FirstOrDefault(w => w.Id == weeklyMenuId);

        if (weeklyMenu == null)
            return NotFound("Бизнес-ланч не найден");

        var allDishes = _context.Dishes.ToList();
        var availableDishes = allDishes
            .Where(d => !weeklyMenu.Dishes.Any(wd => wd.Id == d.Id))
            .ToList();

        return Ok(availableDishes);
    }

    // Добавить новую карточку недельного меню
    [HttpPost]
    public IActionResult AddWeeklyMenu([FromBody] WeeklyMenuCard weeklyMenu)
    {
        if (weeklyMenu == null || string.IsNullOrEmpty(weeklyMenu.Name))
        {
            return BadRequest("Карточка меню должна содержать название.");
        }

        _context.WeeklyMenuCards.Add(weeklyMenu);
        _context.SaveChanges();

        return CreatedAtAction(nameof(GetWeeklyMenus), new { id = weeklyMenu.Id }, weeklyMenu);
    }

    // Добавить блюдо в карточку меню
    [HttpPost("{menuId}/add-dish")]
    public IActionResult AddDishToMenu(int menuId, [FromBody] int dishId)
    {
        var menu = _context.WeeklyMenuCards.Include(w => w.Dishes).FirstOrDefault(w => w.Id == menuId);
        if (menu == null) return NotFound("Карточка меню не найдена.");

        var dish = _context.Dishes.Find(dishId);
        if (dish == null) return NotFound("Блюдо не найдено.");

        if (!menu.Dishes.Any(d => d.Id == dishId))
        {
            menu.Dishes.Add(dish);
            _context.SaveChanges();
        }

        return Ok(menu);
    }
    // Удалить блюдо из карточки меню
    [HttpDelete("{menuId}/remove-dish/{dishId}")]
    public IActionResult RemoveDishFromMenu(int menuId, int dishId)
    {
        var menu = _context.WeeklyMenuCards.Include(w => w.Dishes).FirstOrDefault(w => w.Id == menuId);
        if (menu == null) return NotFound("Карточка меню не найдена.");

        var dish = menu.Dishes.FirstOrDefault(d => d.Id == dishId);
        if (dish == null) return NotFound("Блюдо не найдено в этом меню.");

        menu.Dishes.Remove(dish);
        _context.SaveChanges();

        return Ok(menu);
    }

    // Редактировать название карточки меню
    [HttpPut("{id}")]
    public IActionResult EditWeeklyMenu(int id, [FromBody] WeeklyMenuCard updatedMenu)
    {
        var menu = _context.WeeklyMenuCards.FirstOrDefault(w => w.Id == id);

        if (menu == null)
            return NotFound("Карточка меню не найдена.");

        if (!string.IsNullOrEmpty(updatedMenu.Name))
        {
            menu.Name = updatedMenu.Name;
            _context.SaveChanges();
        }

        return Ok(new { Message = "Карточка меню успешно обновлена." });
    }

    // Удалить карточку недельного меню
    [HttpDelete("{id}")]
    public IActionResult DeleteWeeklyMenu(int id)
    {
        var menu = _context.WeeklyMenuCards
            .Include(w => w.Dishes) // Подгружаем список блюд
            .FirstOrDefault(w => w.Id == id);

        if (menu == null)
            return NotFound("Карточка меню не найдена.");

        _context.WeeklyMenuCards.Remove(menu);
        _context.SaveChanges();

        return Ok(new { Message = "Карточка недельного меню успешно удалена." });
    }
}
