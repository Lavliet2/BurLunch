namespace BurLunch.AuthAPI.Models
{
    public class Table
    {
        public int Id { get; set; } // ID стола
        public int Seats { get; set; } // Количество мест
        public string Description { get; set; } // Описание (например, "Стол 1 из 4 мест")
    }

}
