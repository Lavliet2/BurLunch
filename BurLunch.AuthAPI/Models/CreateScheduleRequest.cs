namespace BurLunch.AuthAPI.Models
{
    public class CreateScheduleRequest
    {
        public DateOnly Date { get; set; }
        public int WeeklyMenuId { get; set; }
    }

}
