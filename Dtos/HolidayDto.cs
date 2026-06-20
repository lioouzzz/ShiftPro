using System.Text.Json.Serialization;

namespace ShiftPro.Dtos
{
    public class HolidayDto
    {
        [JsonPropertyName("Subject")]
        public string Subject { get; set; } = "";

        [JsonPropertyName("Start Date")]
        public string StartDate { get; set; } 

    }
}
