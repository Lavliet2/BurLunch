namespace BurLunch.WebApp.Models
{
        public class RawDish
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public int DishTypeId { get; set; }
            public string DishType { get; set; } // Это строка из JSON
        }
        public class RawWeeklyMenu
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public List<RawDish> Dishes { get; set; }
        }
}
