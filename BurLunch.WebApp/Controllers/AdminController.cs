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
    //public class RawDish
    //{
    //    public int Id { get; set; }
    //    public string Name { get; set; }
    //    public string Description { get; set; }
    //    public int DishTypeId { get; set; }
    //    public string DishType { get; set; } // Это строка из JSON
    //}

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
    public async Task<IActionResult> AddWeeklyMenu()
    {
        var client = _httpClientFactory.CreateClient("BurLunchAPI");
        var response = await client.PostAsync("WeeklyMenu", null);

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


}
