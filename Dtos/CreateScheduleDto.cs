namespace ShiftPro.Dtos
{
    public class CreateScheduleDto
    {
        public int EmployeeId { get; set; }

        public DateOnly WorkDate { get; set; }
    }
}
