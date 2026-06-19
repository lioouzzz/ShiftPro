namespace ShiftPro.Dtos
{
    public class EmployeeReportDto
    {
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;

        public int MonthlyWorkDays { get; set; }
        public int YearlyWorkDays { get; set; }
    }
}
