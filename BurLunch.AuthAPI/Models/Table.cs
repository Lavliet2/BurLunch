namespace BurLunch.AuthAPI.Models
{
    public class Table
    {
        public int Id { get; set; } // ID стола
        public int Seats { get; set; } // Количество мест
        public string Description { get; set; } // Описание (например, "Стол 1 из 4 мест")
    }
    public class TableReservation
    {
        public int Id { get; set; } // ID записи брони
        public int TableId { get; set; } // ID стола
        public Table Table { get; set; } // Связь с таблицей столов

        public int UserId { get; set; } // ID пользователя
        public User User { get; set; } // Связь с моделью пользователя
    }
}
