namespace BurLunch.AuthAPI.Models
{
    public class WeeklyMenuCard
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<Dish> Dishes { get; set; } = new List<Dish>();
    }

}
