using BurLunch.AuthAPI.Models;
namespace BurLunch.WebApp.Models
{
        public class RawDish
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public int DishTypeId { get; set; }
            public string DishType { get; set; }
        }
        public class RawWeeklyMenu
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public List<RawDish> Dishes { get; set; }
        }

        public class MenuWithTablesViewModel
        {

            public RawWeeklyMenu Menu { get; set; }
            public List<Table> Tables { get; set; }
            public int ScheduleId { get; set; }
            public List<RawReservation> Reservations { get; set; }
        }


        public class RawReservation
        {
            public int Id { get; set; }
            public int ScheduleId { get; set; }
            public int TableId { get; set; }
            public RawTable Table { get; set; }
            public int UserId { get; set; }
            public string UserName { get; set; }
            public int SeatsReserved { get; set; }
            public DateTime ReservationTime { get; set; }
            public List<RawDish> SelectedDishes { get; set; }
        }

        public class RawTable
        {
            public int Id { get; set; }
            public int Seats { get; set; }
            public string Description { get; set; }
        }


        public class RawSelection
        {
            public List<int> DishIds { get; set; }
            public int TableId { get; set; }
            public int ScheduleId { get; set; }
            public int UserId { get; set; }
            public int SeatsReserved { get; set; }
            public DateTime Date { get; set; }
        }

        public class SaveSelectionRequest
        {
            public int ScheduleId { get; set; }
            public int TableId { get; set; }
            public List<int> DishIds { get; set; }
            public int SeatsReserved { get; set; }
        }

}
