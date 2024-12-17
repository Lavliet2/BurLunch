namespace BurLunch.AuthAPI.Models
{
    public class WeeklyMenuCard
    {
        public int Id { get; set; } // Уникальный идентификатор карточки меню
        public string Name { get; set; } // Название карточки меню (например, "Бизнес-ланч #1")

        // Список блюд для этого меню
        public ICollection<Dish> Dishes { get; set; } = new List<Dish>();
    }

}
