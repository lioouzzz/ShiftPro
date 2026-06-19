namespace ShiftPro.Dtos
{
    public class HolidayDto
    {
        public DateOnly Date { get; set; }

        public bool IsHoliday { get; set; }

        public string HolidayName { get; set; } = string.Empty;
    }
}
