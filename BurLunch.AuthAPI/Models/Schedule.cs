namespace BurLunch.AuthAPI.Models
{
    public class Schedule
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int WeeklyMenuId { get; set; }
        public WeeklyMenuCard WeeklyMenu { get; set; } 
        public ICollection<TableReservation> TableReservations { get; set; } = new List<TableReservation>();
    }
}
