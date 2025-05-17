using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;

namespace InventoryAppDomainLayer.Common
{
    public class CustomDateTimeConverter : JsonConverter<DateTime>
    {
        private const string DateFormat1 = "yyyy-MM-dd HH:mm:ss.fffffff"; // First format
        private const string DateFormat2 = "yyyy-MM-ddTHH:mm:ss.fffZ";    // Second format with 'T' and 'Z'


        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var dateString = reader.GetString();

            if (DateTime.TryParseExact(dateString, DateFormat1, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out var date))
            {
                return date;
            }

            if (DateTime.TryParseExact(dateString, DateFormat2, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out date))
            {
                return date;
            }

            throw new JsonException($"Unable to parse date: {dateString}");
        }
        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString(DateFormat1));
        }
    }
}
