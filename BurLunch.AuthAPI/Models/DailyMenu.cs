namespace BurLunch.AuthAPI.Models
{
    public class DailyMenu
    {
        public int Id { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
        public int WeeklyMenuCardId { get; set; }
        public WeeklyMenuCard WeeklyMenuCard { get; set; }
        public ICollection<Dish> Dishes { get; set; } = new List<Dish>();
    }

}
