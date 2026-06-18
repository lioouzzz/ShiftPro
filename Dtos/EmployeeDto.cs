using ShiftPro.Enums;

namespace ShiftPro.Dtos
{
    public class EmployeeDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";

        public Role Role { get; set; }
        public bool IsActived { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
