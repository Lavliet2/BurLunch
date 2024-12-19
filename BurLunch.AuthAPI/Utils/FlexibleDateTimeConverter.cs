using System.Text.Json.Serialization;
using System.Text.Json;

namespace BurLunch.AuthAPI.Utils
{
    public class FlexibleDateTimeConverter : JsonConverter<DateTime>
    {
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var dateString = reader.GetString();

            if (string.IsNullOrEmpty(dateString))
                throw new JsonException("Дата не может быть пустой.");

            // Попробуем несколько форматов
            if (DateTime.TryParse(dateString, out var parsedDate))
                return DateTime.SpecifyKind(parsedDate, DateTimeKind.Utc); // Всегда возвращаем дату в UTC

            // Если формат не подходит, выбрасываем исключение
            throw new JsonException($"Неверный формат даты: {dateString}");
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToUniversalTime().ToString("o")); // Записываем дату в формате ISO 8601 в UTC
        }
    }
}
