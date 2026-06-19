namespace ShiftPro.Dtos
{
    public class ScheduleDto
    {
        public int Id { get; set; }

        public int EmployeeId { get; set; }
        public string? EmployeeName { get; set; }
        public DateOnly WorkDate { get; set; }
    }
}
