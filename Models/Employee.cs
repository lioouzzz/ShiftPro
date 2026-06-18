using ShiftPro.Enums;
using System.ComponentModel.DataAnnotations;

namespace ShiftPro.Models
{
    public class Employee
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(20)]
        public string Name { get; set; } = "";
        public Role Role { get; set; } = Role.Employee;
        public bool IsActived { get; set; } = true;
        public DateTime CreatedAt { get; set; } 

        public List<Schedule> Schedules { get; set; } = new();
    }
}
