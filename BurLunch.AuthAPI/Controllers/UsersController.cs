using BurLunch.AuthAPI.Data;
using BurLunch.AuthAPI.Models;
using Microsoft.AspNetCore.Mvc;
using BurLunch.AuthAPI.Services;
using BurLunch.AuthAPI.Utils;

[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly AuthenticationService _authService;

    public UsersController(ApplicationDbContext context, AuthenticationService authService)
    {
        _context = context;
        _authService = authService;
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        if (request == null || string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
        {
            return BadRequest("Запрос на вход некорректен.");
        }

        var user = _context.Users.FirstOrDefault(u => u.Username == request.Username);
        if (user == null || !_authService.VerifyPassword(request.Password, user.PasswordHash))
        {
            return Unauthorized(new { Message = "Неправильное имя пользователя или пароль" });
        }

        // Возвращаем только необходимые данные пользователя
        return Ok(new
        {
            user.Id,
            user.Username,
            user.Role
        });
    }

    [HttpGet]
    public IActionResult GetUsers()
    {
        var users = _context.Users
            .Select(u => new
            {
                u.Id,
                u.Username,
                u.Role
            })
            .ToList();

        return Ok(users);
    }

    [HttpGet("{id}")]
    public IActionResult GetUserById(int id)
    {
        var user = _context.Users
            .Where(u => u.Id == id)
            .Select(u => new
            {
                u.Id,
                u.Username,
                u.Role
            })
            .FirstOrDefault();

        if (user == null)
        {
            return NotFound("Пользователь не найден.");
        }

        return Ok(user);
    }

    [HttpPost]
    public IActionResult CreateUser([FromBody] User user)
    {
        if (user == null || user.Id <= 0 || string.IsNullOrEmpty(user.Username) || string.IsNullOrEmpty(user.Role))
        {
            return BadRequest("Все поля должны быть заполнены.");
        }

        // Проверяем, существует ли пользователь с таким именем
        if (_context.Users.Any(u => u.Username == user.Username))
        {
            return Conflict("Пользователь с таким именем уже существует.");
        }

        if (string.IsNullOrEmpty(user.PasswordHash))
        {
            user.PasswordHash = user.Id.ToString();
        }
        user.PasswordHash = PasswordHasher.HashPassword(user.PasswordHash);

        _context.Users.Add(user);
        _context.SaveChanges();

        return CreatedAtAction(nameof(GetUsers), new { id = user.Id }, new
        {
            user.Id,
            user.Username,
            user.Role
        });
    }


    [HttpDelete("{id}")]
    public IActionResult DeleteUser(int id)
    {
        var user = _context.Users.FirstOrDefault(u => u.Id == id);

        if (user == null)
        {
            return NotFound("Пользователь не найден.");
        }

        if (user.Username == "Admin")
        {
            return BadRequest("Удаление администратора запрещено.");
        }

        _context.Users.Remove(user);
        _context.SaveChanges();

        return Ok(new { Message = "Пользователь успешно удалён." });
    }
}
