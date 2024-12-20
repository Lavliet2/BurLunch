using BurLunch.AuthAPI.Models;
using BurLunch.WebApp.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

public class MenuController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;

    public MenuController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    //public async Task<IActionResult> Menu(DateTime? date = null)
    //{
    //    var client = _httpClientFactory.CreateClient("BurLunchAPI");

    //    // Если дата не указана, берем текущую
    //    var targetDate = date ?? DateTime.UtcNow.Date;

    //    // Запрос расписания на указанную дату
    //    var response = await client.GetAsync($"Schedule/ByDate?date={targetDate:yyyy-MM-dd}");

    //    if (response.IsSuccessStatusCode)
    //    {
    //        var schedule = JsonSerializer.Deserialize<Schedule>(
    //            await response.Content.ReadAsStringAsync(),
    //            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
    //        );

    //        if (schedule != null && schedule.WeeklyMenu != null)
    //        {
    //            ViewBag.Date = targetDate;
    //            return View("Menu", schedule.WeeklyMenu); // Передаём WeeklyMenu в представление
    //        }
    //    }

    //    // Если расписание не найдено
    //    ViewBag.Message = $"Расписание на {targetDate:dd.MM.yyyy} не создано.";
    //    ViewBag.Date = targetDate;
    //    return View("NoSchedule");
    //}

    //public async Task<IActionResult> Menu(DateTime? date = null)
    //{
    //    var client = _httpClientFactory.CreateClient("BurLunchAPI");

    //    // Если дата не указана, берем текущую
    //    var targetDate = date ?? DateTime.UtcNow.Date;

    //    // Запрос расписания на указанную дату
    //    var response = await client.GetAsync($"Schedule/ByDate?date={targetDate:yyyy-MM-dd}");

    //    if (response.IsSuccessStatusCode)
    //    {
    //        var schedule = JsonSerializer.Deserialize<Schedule>(
    //            await response.Content.ReadAsStringAsync(),
    //            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
    //        );

    //        if (schedule != null && schedule.WeeklyMenu != null)
    //        {
    //            ViewBag.Date = targetDate;
    //            return View("Menu", schedule.WeeklyMenu); // Передаём WeeklyMenu в представление
    //        }
    //    }

    //    // Если расписание не найдено
    //    ViewBag.Message = $"Расписание на {targetDate:dd.MM.yyyy} не создано.";
    //    ViewBag.Date = targetDate;
    //    return View("NoSchedule");
    //}

    public async Task<IActionResult> Menu(DateTime? date = null)
    {
        var client = _httpClientFactory.CreateClient("BurLunchAPI");
        var targetDate = date ?? DateTime.UtcNow.Date;

        // Запрос расписания
        var response = await client.GetAsync($"Schedule/ByDate?date={targetDate:yyyy-MM-dd}");

        if (response.IsSuccessStatusCode)
        {
            var schedule = JsonSerializer.Deserialize<Schedule>(
                await response.Content.ReadAsStringAsync(),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );

            if (schedule != null && schedule.WeeklyMenu != null)
            {
                // Если блюда в меню не загружены, запросим их отдельно
                if (schedule.WeeklyMenu.Dishes == null || !schedule.WeeklyMenu.Dishes.Any())
                {
                    var dishesResponse = await client.GetAsync($"WeeklyMenu/{schedule.WeeklyMenu.Id}/Dishes");
                    if (dishesResponse.IsSuccessStatusCode)
                    {
                        schedule.WeeklyMenu.Dishes = JsonSerializer.Deserialize<List<Dish>>(
                            await dishesResponse.Content.ReadAsStringAsync(),
                            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                        );
                    }
                }

                ViewBag.Date = targetDate;
                return View("Menu", schedule.WeeklyMenu); // Передаём меню в представление
            }
        }

        ViewBag.Message = $"Расписание на {targetDate:dd.MM.yyyy} не создано.";
        ViewBag.Date = targetDate;
        return View("NoSchedule");
    }



    [HttpGet("Menu/{date?}")]
    public async Task<IActionResult> MenuByDate(string? date)
    {
        DateTime parsedDate;
        if (string.IsNullOrEmpty(date) || !DateTime.TryParse(date, out parsedDate))
        {
            parsedDate = DateTime.UtcNow.Date; // Если дата не указана или неверная, используем текущую
        }

        // Просто вызываем метод Menu, чтобы он обработал логику
        return await Menu(parsedDate);
    }
}

