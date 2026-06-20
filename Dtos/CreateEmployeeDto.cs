using ShiftPro.Enums;
using ShiftPro.Models;

namespace ShiftPro.Dtos
{
    public class CreateEmployeeDto
    {
        public string Account { get; set; } = "";
        public string Name { get; set; } = "";
        public string Password { get; set; } = "";
        public Role Role { get; set; }
    }
}