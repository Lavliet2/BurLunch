using BurLunch.AuthAPI.Data;
using Microsoft.AspNetCore.Mvc;
using BurLunch.AuthAPI.Models;

[Route("api/[controller]")]
[ApiController]
public class TablesController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public TablesController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult GetTables()
    {
        return Ok(_context.Tables.ToList());
    }

    [HttpGet("{id}")]
    public IActionResult GetTableById(int id)
    {
        var table = _context.Tables.Find(id);

        if (table == null)
            return NotFound($"Стол с ID {id} не найден.");

        return Ok(table);
    }


    [HttpPost]
    public IActionResult AddTable([FromBody] Table table)
    {
        if (table == null || table.Seats <= 0)
            return BadRequest("Некорректные данные стола");

        _context.Tables.Add(table);
        _context.SaveChanges();
        return Ok(table);
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteTable(int id)
    {
        var table = _context.Tables.Find(id);
        if (table == null)
            return NotFound($"Стол с ID {id} не найден.");

        _context.Tables.Remove(table);
        _context.SaveChanges();
        return Ok(new { message = $"Стол с ID {id} успешно удалён." });
    }
}
