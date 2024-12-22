namespace BurLunch.AuthAPI.Models
{
    public class TableReservation
    {
        public int Id { get; set; } 
        public int ScheduleId { get; set; }
        public Schedule Schedule { get; set; }
        public int TableId { get; set; }
        public Table Table { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public int SeatsReserved { get; set; }
        public DateTime ReservationTime { get; set; }
        public ICollection<Dish> SelectedDishes { get; set; } = new List<Dish>();
    }
}
