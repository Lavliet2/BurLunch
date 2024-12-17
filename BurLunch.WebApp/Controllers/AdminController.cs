using BurLunch.AuthAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text;
using System.Text.Json;

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
    public class RawDish
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int DishTypeId { get; set; }
        public string DishType { get; set; } // Это строка из JSON
    }

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

























    //public async Task<IActionResult> GetDishes()
    //{
    //    var client = _httpClientFactory.CreateClient("BurLunchAPI");

    //    // Запрос списка блюд
    //    var responseDishes = await client.GetAsync("Dishes");
    //    var responseDishTypes = await client.GetAsync("DishTypes");

    //    if (responseDishes.IsSuccessStatusCode && responseDishTypes.IsSuccessStatusCode)
    //    {
    //        var rawDishes = JsonSerializer.Deserialize<List<RawDish>>(
    //            await responseDishes.Content.ReadAsStringAsync(),
    //            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
    //        );

    //        var dishTypes = JsonSerializer.Deserialize<List<DishType>>(
    //            await responseDishTypes.Content.ReadAsStringAsync(),
    //            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
    //        );

    //        ViewBag.DishTypes = dishTypes;

    //        var dishes = rawDishes.Select(rd => new Dish
    //        {
    //            Id = rd.Id,
    //            Name = rd.Name,
    //            Description = rd.Description,
    //            DishTypeId = rd.DishTypeId,
    //            DishType = new DishType { Name = rd.DishType }
    //        }).ToList();

    //        return PartialView("_ManageDishesPartial", dishes);
    //    }

    //    ModelState.AddModelError("", "Не удалось загрузить данные.");
    //    return PartialView("_ManageDishesPartial", new List<Dish>());
    //}

    //[HttpPost]
    //public async Task<IActionResult> AddDish([FromBody] Dish dish)
    //{
    //    if (dish == null || string.IsNullOrEmpty(dish.Name) || dish.DishTypeId <= 0)
    //    {
    //        return BadRequest("Название блюда и тип блюда обязательны.");
    //    }

    //    var client = _httpClientFactory.CreateClient("BurLunchAPI");
    //    var jsonContent = new StringContent(JsonSerializer.Serialize(dish), Encoding.UTF8, "application/json");
    //    var response = await client.PostAsync("Dishes", jsonContent);

    //    if (response.IsSuccessStatusCode)
    //    {
    //        return Ok();
    //    }
    //    else
    //    {
    //        return StatusCode((int)response.StatusCode, "Не удалось добавить блюдо.");
    //    }
    //}
    //[HttpPost]
    //public async Task<IActionResult> AddDish([FromBody] Dish dish)
    //{
    //    if (dish == null || string.IsNullOrEmpty(dish.Name) || dish.DishTypeId <= 0)
    //    {
    //        return BadRequest("Название блюда и тип блюда обязательны.");
    //    }

    //    var client = _httpClientFactory.CreateClient("BurLunchAPI");

    //    // Создаём объект для отправки в API
    //    var payload = new
    //    {
    //        name = dish.Name,
    //        description = dish.Description,
    //        dishTypeId = dish.DishTypeId
    //    };

    //    var jsonContent = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

    //    // Отправляем данные в API
    //    var response = await client.PostAsync("Dishes", jsonContent);

    //    if (response.IsSuccessStatusCode)
    //    {
    //        return Ok();
    //    }
    //    else
    //    {
    //        return StatusCode((int)response.StatusCode, "Не удалось добавить блюдо.");
    //    }
    //}




















    //public async Task<IActionResult> GetWeeklyMenus()
    //{
    //    var client = _httpClientFactory.CreateClient("BurLunchAPI");
    //    var response = await client.GetAsync("WeeklyMenu");

    //    if (response.IsSuccessStatusCode)
    //    {
    //        var weeklyMenus = JsonSerializer.Deserialize<List<WeeklyMenuCard>>(
    //            await response.Content.ReadAsStringAsync(),
    //            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
    //        );

    //        return PartialView("_ManageWeeklyMenusPartial", weeklyMenus);
    //    }

    //    ModelState.AddModelError("", "Не удалось загрузить недельные меню.");
    //    return PartialView("_ManageWeeklyMenusPartial", new List<WeeklyMenuCard>());
    //}

    //public async Task<IActionResult> GetTables()
    //{
    //    var client = _httpClientFactory.CreateClient("BurLunchAPI");
    //    var response = await client.GetAsync("Tables");

    //    if (response.IsSuccessStatusCode)
    //    {
    //        var tables = JsonSerializer.Deserialize<List<Table>>(
    //            await response.Content.ReadAsStringAsync(),
    //            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
    //        );

    //        return PartialView("_ManageTablesPartial", tables);
    //    }

    //    ModelState.AddModelError("", "Не удалось загрузить список столов.");
    //    return PartialView("_ManageTablesPartial", new List<Table>());
    //}


}
