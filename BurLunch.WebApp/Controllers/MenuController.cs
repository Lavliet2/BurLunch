using BurLunch.AuthAPI.Models;
using BurLunch.WebApp.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

public class MenuController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;

    public MenuController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<IActionResult> MenuWithTables(DateTime? date = null)
    {
        var client = _httpClientFactory.CreateClient("BurLunchAPI");

        var targetDate = date ?? DateTime.UtcNow.Date;

        var scheduleResponse = await client.GetAsync($"Schedule/ByDate?date={targetDate:yyyy-MM-dd}");
        if (!scheduleResponse.IsSuccessStatusCode)
        {
            ViewBag.Message = $"Расписание на {targetDate:dd.MM.yyyy} не создано.";
            ViewBag.Date = targetDate;
            return View("NoSchedule");
        }

        var schedule = JsonSerializer.Deserialize<Schedule>(
            await scheduleResponse.Content.ReadAsStringAsync(),
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        );

        if (schedule == null || schedule.WeeklyMenuId <= 0)
        {
            ViewBag.Message = $"Расписание на {targetDate:dd.MM.yyyy} не создано.";
            ViewBag.Date = targetDate;
            return View("NoSchedule");
        }

        var menuResponse = await client.GetAsync($"WeeklyMenu/{schedule.WeeklyMenuId}");
        if (!menuResponse.IsSuccessStatusCode)
        {
            ViewBag.Message = $"Меню для расписания на {targetDate:dd.MM.yyyy} не найдено.";
            ViewBag.Date = targetDate;
            return View("NoSchedule");
        }

        var weeklyMenuCard = JsonSerializer.Deserialize<RawWeeklyMenu>(
            await menuResponse.Content.ReadAsStringAsync(),
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        );

        var tablesResponse = await client.GetAsync($"Tables?date={targetDate:yyyy-MM-dd}");
        var tables = new List<Table>();
        if (tablesResponse.IsSuccessStatusCode)
        {
            tables = JsonSerializer.Deserialize<List<Table>>(
                await tablesResponse.Content.ReadAsStringAsync(),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );
        }

        var reservationsResponse = await client.GetAsync($"/api/TableReservation/reservations/{schedule.Id}");
        var reservations = new List<RawReservation>();
        if (reservationsResponse.IsSuccessStatusCode)
        {
            reservations = JsonSerializer.Deserialize<List<RawReservation>>(
                await reservationsResponse.Content.ReadAsStringAsync(),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );
        }

        var viewModel = new MenuWithTablesViewModel
        {
            Menu = weeklyMenuCard,
            Tables = tables,
            ScheduleId = schedule.Id,
            Reservations = reservations
        };


        ViewBag.Date = targetDate;
        return View("Menu", viewModel);
    }


    [HttpPost]
    public async Task<IActionResult> ReserveTable([FromBody] TableReservation request)
    {
        var client = _httpClientFactory.CreateClient("BurLunchAPI");

        var jsonContent = new StringContent(
            JsonSerializer.Serialize(request),
            Encoding.UTF8,
            "application/json"
        );

        var response = await client.PostAsync("TableReservations", jsonContent);

        if (response.IsSuccessStatusCode)
        {
            return Ok(new { Message = "Стол успешно забронирован!" });
        }
        else
        {
            return StatusCode((int)response.StatusCode, "Ошибка при бронировании стола.");
        }
    }


    [HttpGet("Menu/{date?}")]
    public async Task<IActionResult> MenuByDate(string? date)
    {
        DateTime parsedDate;
        if (string.IsNullOrEmpty(date) || !DateTime.TryParse(date, out parsedDate))
        {
            parsedDate = DateTime.UtcNow.Date;
        }

        return await MenuWithTables(parsedDate);
    }


    [HttpPost]
    public async Task<IActionResult> SaveSelection([FromBody] SaveSelectionRequest request)
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
        {
            return Unauthorized("Пользователь не аутентифицирован.");
        }

        var reservationData = new
        {
            UserId = int.Parse(userId),
            TableId = request.TableId,
            ScheduleId = request.ScheduleId,
            DishIds = request.DishIds,
            SeatsReserved = request.SeatsReserved
        };

        var client = _httpClientFactory.CreateClient("BurLunchAPI");

        var jsonContent = new StringContent(
            JsonSerializer.Serialize(reservationData),
            Encoding.UTF8,
            "application/json"
        );

        var response = await client.PostAsync("TableReservation/book", jsonContent);

        if (response.IsSuccessStatusCode)
        {
            return Ok(new { Message = "Выбор успешно сохранён!" });
        }

        var errorMessage = await response.Content.ReadAsStringAsync();
        return StatusCode((int)response.StatusCode, errorMessage);
    }

}
