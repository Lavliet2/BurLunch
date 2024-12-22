using BurLunch.AuthAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using BurLunch.WebApp.Models;

[Authorize(Roles = "Administrator")]
public class AdminController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;

    public AdminController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    /***** Управление пользователями *****/
    public async Task<IActionResult> ManageUsers()
    {
        var client = _httpClientFactory.CreateClient("BurLunchAPI");
        var response = await client.GetAsync("Users");

        if (response.IsSuccessStatusCode)
        {
            var users = JsonSerializer.Deserialize<List<User>>(
                await response.Content.ReadAsStringAsync(),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );

            return View(users);
        }

        ModelState.AddModelError("", "Не удалось загрузить список пользователей.");
        return View(new List<User>());
    }

    [HttpGet]
    public IActionResult AddUser()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> AddUser([FromBody] User user)
    {
        if (user == null || user.Id <= 0 || string.IsNullOrEmpty(user.Username) || string.IsNullOrEmpty(user.Role))
        {
            TempData["Error"] = "Все поля должны быть заполнены.";
            return RedirectToAction("ManageUsers");
        }
        user.PasswordHash = user.Id.ToString();

        var client = _httpClientFactory.CreateClient("BurLunchAPI");
        var jsonContent = new StringContent(JsonSerializer.Serialize(user), Encoding.UTF8, "application/json");

        var response = await client.PostAsync("Users", jsonContent);

        if (response.IsSuccessStatusCode)
        {
            TempData["Message"] = "Пользователь успешно добавлен.";
        }
        else
        {
            TempData["Error"] = "Не удалось добавить пользователя.";
        }

        return RedirectToAction("ManageUsers");
    }


    [HttpPost]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var client = _httpClientFactory.CreateClient("BurLunchAPI");
        var response = await client.DeleteAsync($"Users/{id}");

        if (response.IsSuccessStatusCode)
        {
            TempData["Message"] = "Пользователь успешно удалён.";
        }
        else
        {
            var errorMessage = await response.Content.ReadAsStringAsync();
            TempData["Error"] = $"Не удалось удалить пользователя. {errorMessage}";
        }

        return RedirectToAction("ManageUsers");
    }
    /*****************************************************************************************/
                                /***** Управление меню*****/
    /***** Управление блюдами*****/
    public IActionResult ManageMenu()
    {
        return View();
    }

    public async Task<IActionResult> ManageDishes()
    {
        var client = _httpClientFactory.CreateClient("BurLunchAPI");
        var responseDishes = await client.GetAsync("Dishes");
        var responseDishTypes = await client.GetAsync("DishTypes");

        if (responseDishes.IsSuccessStatusCode && responseDishTypes.IsSuccessStatusCode)
        {
            var rawDishes = JsonSerializer.Deserialize<List<RawDish>>(
                await responseDishes.Content.ReadAsStringAsync(),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );

            var dishTypes = JsonSerializer.Deserialize<List<DishType>>(
                await responseDishTypes.Content.ReadAsStringAsync(),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );

            ViewBag.DishTypes = dishTypes;

            var dishes = rawDishes.Select(rd => new Dish
            {
                Id = rd.Id,
                Name = rd.Name,
                Description = rd.Description,
                DishTypeId = rd.DishTypeId,
                DishType = new DishType { Name = rd.DishType }
            }).ToList();

            return View(dishes);
        }

        ModelState.AddModelError("", "Не удалось загрузить данные.");
        return View(new List<Dish>());
    }

    [HttpPost]
    public async Task<IActionResult> AddDish([FromBody] Dish dish)
    {
        if (dish == null || string.IsNullOrEmpty(dish.Name) || dish.DishTypeId <= 0)
        {
            return BadRequest("Название блюда и тип блюда обязательны.");
        }

        try
        {
            var client = _httpClientFactory.CreateClient("BurLunchAPI");

            var payload = new
            {
                name = dish.Name,
                description = dish.Description,
                dishTypeId = dish.DishTypeId
            };

            var jsonContent = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            var response = await client.PostAsync("Dishes", jsonContent);

            if (response.IsSuccessStatusCode || response.ReasonPhrase == "Internal Server Error")
            {
                return Ok(new { message = "Блюдо успешно добавлено" });
            }
            else
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                return StatusCode((int)response.StatusCode, $"Ошибка при добавлении блюда: {errorMessage}");
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Произошла ошибка: {ex.Message}");
        }
    }

    [HttpPost]
    public async Task<IActionResult> DeleteDish(int id)
    {
        if (id <= 0)
        {
            TempData["Error"] = "Некорректный идентификатор блюда.";
            return RedirectToAction("ManageDishes");
        }

        var client = _httpClientFactory.CreateClient("BurLunchAPI");
        var response = await client.DeleteAsync($"Dishes/{id}");

        if (response.IsSuccessStatusCode)
        {
            TempData["Message"] = "Блюдо успешно удалено.";
        }
        else
        {
            TempData["Error"] = "Не удалось удалить блюдо. Попробуйте ещё раз.";
        }

        return RedirectToAction("ManageDishes");
    }
    /*****************************************************************************************/
    /***** Управление столами*****/
    [HttpGet]
    public async Task<IActionResult> ManageTables()
    {
        var client = _httpClientFactory.CreateClient("BurLunchAPI");
        var response = await client.GetAsync("Tables");

        if (response.IsSuccessStatusCode)
        {
            var tables = JsonSerializer.Deserialize<List<Table>>(
                await response.Content.ReadAsStringAsync(),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );

            return View(tables);
        }

        ModelState.AddModelError("", "Не удалось загрузить список столов.");
        return View(new List<Table>());
    }

    [HttpPost]
    public async Task<IActionResult> AddTable([FromBody] Table table)
    {
        if (table == null || table.Id <= 0 || table.Seats <= 0)
        {
            return BadRequest("Номер стола и количество мест обязательны для заполнения.");
        }

        var client = _httpClientFactory.CreateClient("BurLunchAPI");
        var jsonContent = new StringContent(JsonSerializer.Serialize(table), Encoding.UTF8, "application/json");

        var response = await client.PostAsync("Tables", jsonContent);

        if (response.IsSuccessStatusCode)
        {
            return Ok(new { message = "Стол успешно добавлен." });
        }

        return StatusCode((int)response.StatusCode, "Не удалось добавить стол.");
    }


    [HttpPost]
    public async Task<IActionResult> DeleteTable(int id)
    {
        var client = _httpClientFactory.CreateClient("BurLunchAPI");
        var response = await client.DeleteAsync($"Tables/{id}");

        if (response.IsSuccessStatusCode)
        {
            TempData["Message"] = "Стол успешно удалён.";
        }
        else
        {
            TempData["Error"] = "Не удалось удалить стол.";
        }

        return RedirectToAction("ManageTables");
    }
    /*****************************************************************************************/
    /***** Управление недельным меню*****/
    public async Task<IActionResult> ManageWeeklyMenu()
    {
        var client = _httpClientFactory.CreateClient("BurLunchAPI");
        var response = await client.GetAsync("WeeklyMenu");

        if (response.IsSuccessStatusCode)
        {
            var weeklyMenus = JsonSerializer.Deserialize<List<RawWeeklyMenu>>(
                await response.Content.ReadAsStringAsync(),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );

            return View(weeklyMenus);
        }

        ModelState.AddModelError("", "Не удалось загрузить недельное меню.");
        return View(new List<RawWeeklyMenu>());
    }

    // Добавление новой карточки меню
    [HttpPost]
    public async Task<IActionResult> AddWeeklyMenu(string menuName)
    {
        var client = _httpClientFactory.CreateClient("BurLunchAPI");

        var payload = new { Name = menuName };
        var jsonContent = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

        var response = await client.PostAsync("WeeklyMenu", jsonContent);

        if (response.IsSuccessStatusCode)
        {
            TempData["Message"] = "Новая карточка бизнес-ланча успешно создана.";
        }
        else
        {
            TempData["Error"] = "Не удалось создать карточку бизнес-ланча.";
        }

        return RedirectToAction("ManageWeeklyMenu");
    }

    // Удаление карточки меню
    [HttpPost]
    public async Task<IActionResult> DeleteWeeklyMenu(int id)
    {
        var client = _httpClientFactory.CreateClient("BurLunchAPI");
        var response = await client.DeleteAsync($"WeeklyMenu/{id}");

        if (response.IsSuccessStatusCode)
        {
            TempData["Message"] = "Карточка меню успешно удалена.";
        }
        else
        {
            TempData["Error"] = "Не удалось удалить карточку меню.";
        }

        return RedirectToAction("ManageWeeklyMenu");
    }

    // Добавление блюда в карточку меню
    [HttpPost]
    public async Task<IActionResult> AddDishToWeeklyMenu(int weeklyMenuId, int dishId)
    {
        var client = _httpClientFactory.CreateClient("BurLunchAPI");
        var payload = new { dishId };
        var jsonContent = new StringContent(dishId.ToString(), Encoding.UTF8, "application/json");

        var response = await client.PostAsync($"WeeklyMenu/{weeklyMenuId}/add-dish", jsonContent);

        if (response.IsSuccessStatusCode)
        {
            TempData["Message"] = "Блюдо успешно добавлено в меню.";
        }
        else
        {
            TempData["Error"] = "Не удалось добавить блюдо.";
        }

        return RedirectToAction("ManageWeeklyMenu");
    }

    // Удаление блюда из карточки меню
    [HttpPost]
    public async Task<IActionResult> RemoveDishFromWeeklyMenu(int weeklyMenuId, int dishId)
    {
        var client = _httpClientFactory.CreateClient("BurLunchAPI");
        var response = await client.DeleteAsync($"WeeklyMenu/{weeklyMenuId}/remove-dish/{dishId}");

        if (response.IsSuccessStatusCode)
        {
            TempData["Message"] = "Блюдо успешно удалено из меню.";
        }
        else
        {
            TempData["Error"] = "Не удалось удалить блюдо.";
        }

        return RedirectToAction("ManageWeeklyMenu");
    }

    [HttpGet]
    public async Task<IActionResult> GetAvailableDishes(int weeklyMenuId)
    {
        var client = _httpClientFactory.CreateClient("BurLunchAPI");
        var response = await client.GetAsync($"WeeklyMenu/{weeklyMenuId}/available-dishes");

        if (response.IsSuccessStatusCode)
        {
            var availableDishes = JsonSerializer.Deserialize<List<RawDish>>(
                await response.Content.ReadAsStringAsync(),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );
            ViewBag.WeeklyMenuId = weeklyMenuId;
            return PartialView("_AvailableDishesPartial", availableDishes);
        }

        return Content("Ошибка при загрузке блюд");
    }
    /*****************************************************************************************/
    /***** Управление рассписанием*****/
    public IActionResult ManageSchedule()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> GetWeeklyMenus()
    {
        var client = _httpClientFactory.CreateClient("BurLunchAPI");
        var response = await client.GetAsync("WeeklyMenu");

        if (response.IsSuccessStatusCode)
        {
            var weeklyMenus = JsonSerializer.Deserialize<List<RawWeeklyMenu>>(
                await response.Content.ReadAsStringAsync(),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );

            return Json(weeklyMenus);
        }

        return StatusCode((int)response.StatusCode, "Ошибка при загрузке меню.");
    }

    [HttpGet]
    public async Task<IActionResult> GetSchedules()
    {
        var client = _httpClientFactory.CreateClient("BurLunchAPI");
        var response = await client.GetAsync("Schedule");

        if (!response.IsSuccessStatusCode)
        {
            return StatusCode((int)response.StatusCode, "Ошибка при загрузке расписания.");
        }

        var schedules = await response.Content.ReadAsStringAsync();
        return Content(schedules, "application/json");
    }


    [HttpPost]
    public async Task<IActionResult> CreateSchedules([FromBody] List<CreateScheduleRequest> schedules)
    {
        if (schedules == null || schedules.Count == 0)
        {
            return BadRequest("Данные для создания расписания отсутствуют.");
        }

        try
        {
            var processedSchedules = schedules.Select(schedule => new CreateScheduleRequest
            {
                Date = DateTime.SpecifyKind(schedule.Date, DateTimeKind.Utc),
                WeeklyMenuId = schedule.WeeklyMenuId
            }).ToList();

            var json = JsonSerializer.Serialize(processedSchedules, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            Console.WriteLine($"Отправляем JSON: {json}");

            var jsonContent = new StringContent(json, Encoding.UTF8, "application/json");

            var client = _httpClientFactory.CreateClient("BurLunchAPI");
            var response = await client.PostAsync("Schedule/BulkCreate", jsonContent);

            if (response.IsSuccessStatusCode)
            {
                return Ok(new { Message = "Расписания успешно созданы." });
            }
            else
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                return StatusCode((int)response.StatusCode, $"Ошибка при создании расписания: {errorMessage}");
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Произошла ошибка: {ex.Message}");
        }
    }

    [HttpPost]
    public async Task<IActionResult> DeleteSchedules([FromBody] List<DateTime> dates)
    {
        if (dates == null || dates.Count == 0)
        {
            return BadRequest("Даты для удаления отсутствуют.");
        }

        try
        {
            var client = _httpClientFactory.CreateClient("BurLunchAPI");
            var jsonContent = new StringContent(
                JsonSerializer.Serialize(dates, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                }),
                Encoding.UTF8,
                "application/json"
            );

            var response = await client.PostAsync("Schedule/BulkDelete", jsonContent);

            if (response.IsSuccessStatusCode)
            {
                return Ok(new { Message = "Расписания успешно удалены." });
            }

            var errorMessage = await response.Content.ReadAsStringAsync();
            return StatusCode((int)response.StatusCode, $"Ошибка при удалении расписания: {errorMessage}");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Произошла ошибка: {ex.Message}");
        }
    }

}
