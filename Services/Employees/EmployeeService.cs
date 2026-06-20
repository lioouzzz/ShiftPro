using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ShiftPro.Data;
using ShiftPro.Dtos;
using ShiftPro.Enums;
using ShiftPro.Helpers;
using ShiftPro.Interfaces;
using ShiftPro.Models;

namespace ShiftPro.Services.Employees
{
    public class EmployeeService:IEmployeeService
    {
        private readonly FileLogger _logger;
        private readonly AppDbContext _context;

        public EmployeeService(FileLogger logger, AppDbContext context) 
        {
            _logger = logger;
            _context = context;
        }

        public async Task<List<EmployeeDto>> GetAllEmployees()
        {
            var employees=await _context.Employees
                                    .OrderBy(x => x.CreatedAt)
                                    .Select(x => new EmployeeDto {
                                        Id = x.Id,
                                        Name = x.Name,
                                        Role=x.Role,
                                        IsActived=x.IsActived,
                                        CreatedAt=x.CreatedAt
                                    }).ToListAsync();

            return employees;
        }

        public async Task<EmployeeDto?> GetEmployeeById(int id)
        {
           return  await _context.Employees
                                    .Where(x => x.Id == id)
                                    .Select(x => new EmployeeDto
                                    {
                                        Id = x.Id,
                                        Name = x.Name,
                                        Role = x.Role,
                                        IsActived = x.IsActived,
                                        CreatedAt = x.CreatedAt
                                    }).FirstOrDefaultAsync();
        }


        public async Task<EmployeeDto> CreateEmployee(CreateEmployeeDto dto)
        {
            var employee = new Employee
            {
                Name = dto.Name,
                Account=dto.Account,
                Role = dto.Role,
                IsActived=true,
                CreatedAt = DateTime.Now
            };

            employee.PasswordHash =
                BCrypt.Net.BCrypt.HashPassword(dto.Password);

            _context.Employees.Add(employee);
            _context.SaveChanges();

            var result = new EmployeeDto
            {
                Name = employee.Name,
                Role = employee.Role,
                IsActived = employee.IsActived,
                CreatedAt = employee.CreatedAt,
            };

            return result;
        }


        public async Task<bool> UpdateEmployee(int id ,UpdateEmployeeDto dto)
        {
           var employee = await _context.Employees
             .FirstOrDefaultAsync(x => x.Id == id);

            if (employee == null)
            {
                _logger.Write(new Log {
                Status=ApiResultStatus.Failed,
                Message="找不到此employeeId"
                });
                return false;
            }

            if (!string.IsNullOrWhiteSpace(dto.Name))
            {
                employee.Name = dto.Name;
            }

            if (dto.IsActived.HasValue)
            {
                employee.IsActived = dto.IsActived.Value;
            }

            await  _context.SaveChangesAsync();

            return true;
        }



        public async Task<bool> DeleteEmployee(int id)
        {
            var employee = await _context.Employees
                                                  .FirstOrDefaultAsync(x => x.Id == id);

            if (employee == null)
                {
                    _logger.Write(new Log
                    {
                        Status = ApiResultStatus.Failed,
                        Message = "找不到此employee"
                    });
                    return false;
                }

            employee.IsActived = false;
             await _context.SaveChangesAsync();

            return true;
        }
    }
}
