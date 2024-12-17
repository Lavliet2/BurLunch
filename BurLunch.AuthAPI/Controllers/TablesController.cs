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

    [HttpPost]
    public IActionResult AddTable([FromBody] Table table)
    {
        if (table == null || table.Seats <= 0)
            return BadRequest("Некорректные данные стола");

        _context.Tables.Add(table);
        _context.SaveChanges();
        return Ok(table);
    }
}
