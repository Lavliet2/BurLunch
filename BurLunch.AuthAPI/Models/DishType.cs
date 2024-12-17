namespace BurLunch.AuthAPI.Models
{
    public class DishType
    {
        public int Id { get; set; } // Уникальный идентификатор типа блюда
        public string Name { get; set; } // Название типа блюда (например, "Салат")

        // Связь с блюдами
        public ICollection<Dish> Dishes { get; set; } = new List<Dish>();
    }

}
