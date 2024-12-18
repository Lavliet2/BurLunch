namespace BurLunch.AuthAPI.Models
{
    public class Schedule
    {
        public int Id { get; set; }
        public DateTime Date { get; set; } // Дата расписания

        public int WeeklyMenuId { get; set; }
        public WeeklyMenuCard WeeklyMenu { get; set; } // Связь с бизнес-ланчем

        public ICollection<TableReservation> TableReservations { get; set; } = new List<TableReservation>();
    }
}
