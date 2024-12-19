namespace BurLunch.AuthAPI.Models
{
    public class CreateScheduleRequest
    {
        public DateTime Date { get; set; }
        public int WeeklyMenuId { get; set; }
    }

}
