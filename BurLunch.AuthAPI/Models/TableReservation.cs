namespace BurLunch.AuthAPI.Models
{
    //public class TableReservation
    //{
    //    public int Id { get; set; } // ID записи брони
    //    public int TableId { get; set; } // ID стола
    //    public Table Table { get; set; } // Связь с таблицей столов

    //    public int UserId { get; set; } // ID пользователя
    //    public User User { get; set; } // Связь с моделью пользователя
    //}
    public class TableReservation
    {
        public int Id { get; set; }

        public int ScheduleId { get; set; }
        public Schedule Schedule { get; set; } // Связь с расписанием.

        public int TableId { get; set; }
        public Table Table { get; set; } // Стол, который был забронирован.

        public int UserId { get; set; }
        public User User { get; set; } // Пользователь, забронировавший места.

        public int SeatsReserved { get; set; } // Количество забронированных мест.

        public DateTime ReservationTime { get; set; } // Время бронирования.
    }
}
