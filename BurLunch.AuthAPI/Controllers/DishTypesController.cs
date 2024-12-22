using BurLunch.AuthAPI.Data;
using BurLunch.AuthAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Route("api/[controller]")]
[ApiController]
public class DishTypesController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public DishTypesController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult GetDishTypes()
    {
        return Ok(_context.DishTypes.ToList());
    }

    [HttpPost]
    public IActionResult AddDishType([FromBody] DishType dishType)
    {
        if (string.IsNullOrEmpty(dishType.Name))
            return BadRequest("Название типа блюда не может быть пустым.");

        _context.DishTypes.Add(dishType);
        _context.SaveChanges();
        return Ok(dishType);
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteDishType(int id)
    {
        var dishType = _context.DishTypes
            .Include(dt => dt.Dishes)
            .FirstOrDefault(dt => dt.Id == id);

        if (dishType == null)
            return NotFound("Тип блюда не найден.");

        // Проверка на связанные блюда
        if (dishType.Dishes.Any())
            return BadRequest("Нельзя удалить тип блюда, если к нему привязаны блюда.");

        _context.DishTypes.Remove(dishType);
        _context.SaveChanges();

        return Ok(new { Message = "Тип блюда успешно удалён." });
    }
}
