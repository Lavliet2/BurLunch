using BurLunch.AuthAPI.Data;
using BurLunch.AuthAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Route("api/[controller]")]
[ApiController]
public class DishesController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public DishesController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult GetDishes()
    {
        var dishes = _context.Dishes
            .Include(d => d.DishType) 
            .Select(d => new
            {
                d.Id,
                d.Name,
                d.Description,
                DishType = d.DishType.Name
            })
            .ToList();

        return Ok(dishes);
    }

    //[HttpGet]
    [HttpGet("filter")]
    public IActionResult GetDishes([FromQuery] int? dishTypeId)
    {
        var query = _context.Dishes.AsQueryable();

        if (dishTypeId.HasValue)
        {
            query = query.Where(d => d.DishTypeId == dishTypeId.Value);
        }

        var dishes = query
            .Include(d => d.DishType)
            .Select(d => new
            {
                d.Id,
                d.Name,
                d.Description,
                DishType = d.DishType.Name
            })
            .ToList();

        return Ok(dishes);
    }

    [HttpPost("findByIds")]
    public IActionResult GetDishesByIds([FromBody] List<int> dishIds)
    {
        if (dishIds == null || !dishIds.Any())
        {
            return BadRequest(new { Message = "Список идентификаторов блюд пуст или отсутствует." });
        }

        var dishes = _context.Dishes
            .Include(d => d.DishType)
            .Where(d => dishIds.Contains(d.Id))
            .Select(d => new
            {
                d.Id,
                d.Name,
                d.Description,
                DishType = d.DishType != null ? d.DishType.Name : null
            })
            .ToList();

        return Ok(dishes);
    }


    [HttpPost]
    public IActionResult AddDish([FromBody] Dish dish)
    {
        if (dish == null || string.IsNullOrEmpty(dish.Name) || dish.DishTypeId <= 0)
        {
            return BadRequest(new { Message = "Некорректные данные блюда." });
        }

        if (_context.Dishes.Any(d => d.Name.ToLower() == dish.Name.ToLower()))
        {
            return Conflict(new { Message = "Блюдо с таким именем уже существует." });
        }

        var dishType = _context.DishTypes.Find(dish.DishTypeId);
        if (dishType == null)
        {
            return NotFound(new { Message = "Тип блюда не найден." });
        }

        _context.Dishes.Add(dish);
        _context.SaveChanges();

        return CreatedAtAction(nameof(AddDish), new { id = dish.Id }, dish);
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteDish(int id)
    {
        var dish = _context.Dishes.FirstOrDefault(d => d.Id == id);

        if (dish == null)
            return NotFound("Блюдо не найдено.");

        _context.Dishes.Remove(dish);
        _context.SaveChanges();

        return Ok(new { Message = "Блюдо успешно удалено." });
    }
}
