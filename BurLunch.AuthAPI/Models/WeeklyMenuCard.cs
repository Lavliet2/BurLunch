namespace BurLunch.AuthAPI.Models
{
    public class WeeklyMenuCard
    {
        public int Id { get; set; }
        public string WeekNumber { get; set; } // Например, "2024-W51"
        public ICollection<DailyMenu> DailyMenus { get; set; } = new List<DailyMenu>();
    }

}
