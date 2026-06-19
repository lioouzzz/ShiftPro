using Microsoft.EntityFrameworkCore;
using ShiftPro.Data;
using ShiftPro.Dtos;
using ShiftPro.Helpers;
using ShiftPro.Interfaces;
using ShiftPro.Models;

namespace ShiftPro.Services.Schedules
{
    public class ScheduleService: IScheduleService
    {
        private readonly FileLogger _logger;
        private readonly AppDbContext _context;

        public ScheduleService(FileLogger logger, AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }


        public async Task<List<ScheduleDto>> GetAllSchedules()
        {
           return  await _context.Schedules
                                    .Include(x => x.Employee)
                                    .Select(x => new ScheduleDto
                                    {
                                        Id = x.Id,
                                        EmployeeId = x.EmployeeId,
                                        EmployeeName=x.Employee.Name,
                                        WorkDate = x.WorkDate,
                                    }).ToListAsync();
        }

        public async Task<ScheduleDto?> GetSchedulesById(int id)
        {
            return await _context.Schedules
                            .Include(x => x.Employee)
                            .Where(x => x.Id == id)
                            .Select(x => new ScheduleDto
                            {
                                Id = x.Id,
                                EmployeeId = x.EmployeeId,
                                EmployeeName = x.Employee.Name,
                                WorkDate = x.WorkDate,
                            }).FirstOrDefaultAsync();
        }

        public async Task<ScheduleDto> CreateSchedule(CreateScheduleDto dto)
        {
            var employeeExist =await  _context.Employees.AnyAsync(x => x.Id == dto.EmployeeId && x.IsActived);

            if (!employeeExist)
            {
                _logger.Write(new Log {
                    Status=ApiResultStatus.Failed,
                    Message="找不到此使用者或為已離職員工"
                });
                return null;
            }

            var schedule = await _context.Schedules.AnyAsync(x => x.EmployeeId == dto.EmployeeId && x.WorkDate == dto.WorkDate);

            if (schedule)
            {
                _logger.Write(new Log
                {
                    Status = ApiResultStatus.Failed,
                    Message = "此員工加入相同日期"
                });
                return null;
            }


            var scheduleData = new Schedule
            {
                EmployeeId = dto.EmployeeId,
                WorkDate = dto.WorkDate,
            };

            _context.Schedules.Add(scheduleData);
            await _context.SaveChangesAsync();

            return new ScheduleDto
            {
                Id = scheduleData.Id,
                EmployeeId = scheduleData.EmployeeId,
                WorkDate = scheduleData.WorkDate
            };
        }

        public async Task<bool> UpdateSchedule(int id, UpdateScheduleDto dto )
        {
            var schedule=await _context.Schedules.FirstOrDefaultAsync(x => x.Id == id && x.EmployeeId==dto.EmployeeId );

            if (schedule == null)
            {
                _logger.Write(new Log
                {
                    Status = ApiResultStatus.Failed,
                    Message = "找不到此使用者"
                });
                return false;
            }

            if (dto.EmployeeId.HasValue)
            {
                var employeeExists = await _context.Employees
                                                                            .AnyAsync(x => x.Id == dto.EmployeeId.Value && x.IsActived);

                if (!employeeExists)
                {
                    _logger.Write(new Log
                    {
                        Status = ApiResultStatus.Failed,
                        Message = "找不到此使用者"
                    });
                    return false;
                }

                schedule.EmployeeId = dto.EmployeeId.Value;
            }

            if (dto.WorkDate.HasValue)
            {
                schedule.WorkDate = dto.WorkDate.Value;
            }

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteSchedule(int id)
        {
            var schedule = await _context.Schedules
                                                              .Where(x=>x.Id==id)
                                                              .FirstOrDefaultAsync();

            if (schedule == null)
            {
                _logger.Write(new Log
                {
                    Status = ApiResultStatus.Failed,
                    Message = "Schedule找不到此Id"
                });
                return false;
            }

            _context.Schedules.Remove(schedule);
            await _context.SaveChangesAsync();
            return true; 
        }


        // 指定查詢月份班表
        public async Task<List<ScheduleDto>> GetMonthlySchedules(int year, int month)
        {
           return  await _context.Schedules
                                    .Include(x => x.Employee)
                                    .Where(x => x.WorkDate.Year == year && x.WorkDate.Month == month)
                                    .OrderBy(x => x.WorkDate)
                                    .ThenBy(x => x.EmployeeId)
                                    .Select(x => new ScheduleDto {
                                        Id = x.Id,
                                        EmployeeId = x.EmployeeId,
                                        EmployeeName = x.Employee.Name,
                                        WorkDate = x.WorkDate
                                    }).ToListAsync();
        }
    }
}
