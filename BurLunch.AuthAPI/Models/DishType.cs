namespace BurLunch.AuthAPI.Models
{
    public class DishType
    {
        public int Id { get; set; }
        public string Name { get; set; } 

        public ICollection<Dish> Dishes { get; set; } = new List<Dish>();
    }

}
