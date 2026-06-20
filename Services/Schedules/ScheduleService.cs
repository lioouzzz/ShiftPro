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
        private readonly IHolidayService _holidayService;

        public ScheduleService(FileLogger logger, AppDbContext context, IHolidayService holidayService)
        {
            _logger = logger;
            _context = context;
            _holidayService = holidayService;
        }


        public async Task<List<ScheduleDto>> GetAllSchedules()
        {
           return  await _context.Schedules
                                    .Include(x => x.Employee)
                                    .OrderBy(x => x.WorkDate)
                                    .Select(x => new ScheduleDto
                                    {
                                        Id = x.Id,
                                        EmployeeId = x.EmployeeId,
                                        EmployeeName=x.Employee.Name,
                                        WorkDate = x.WorkDate,
                                    }).ToListAsync();
        }

        public async Task<List<ScheduleDto?>> GetSchedulesById(int id)
        {
            return await _context.Schedules
                            .Include(x => x.Employee)
                            .Where(x => x.Employee.Id == id)
                            .Select(x => new ScheduleDto
                            {
                                Id = x.Id,
                                EmployeeId = x.EmployeeId,
                                EmployeeName = x.Employee.Name,
                                WorkDate = x.WorkDate,
                            }).ToListAsync();
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


            if (!IsNextMonth(dto.WorkDate))
            {
                _logger.Write(new Log
                {
                    Status = ApiResultStatus.Failed,
                    Message = "只允許添加下個月份的排班"
                });
                return null;
            }

            if (IsWeekend(dto.WorkDate))
            {
                _logger.Write(new Log
                {
                    Status = ApiResultStatus.Failed,
                    Message = "假日不可排班"
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


            //當天排班人數
            var dailyCount = await _context.Schedules.CountAsync(x => x.WorkDate == dto.WorkDate);

            if (dailyCount >= 2)
            {
                _logger.Write(new Log
                {
                    Status = ApiResultStatus.Failed,
                    Message = "當天排班人數已滿"
                });
                return null;
            }

            //排班最多15天
            var monthlyCount = await _context.Schedules
                                                                     .CountAsync(x => x.WorkDate.Year == dto.WorkDate.Year &&
                                                                     x.WorkDate.Month == dto.WorkDate.Month &&
                                                                     x.EmployeeId == dto.EmployeeId);
                                                                     
            if (monthlyCount >= 15)
            {
                _logger.Write(new Log
                {
                    Status = ApiResultStatus.Failed,
                    Message = "員工Id："+ dto.EmployeeId +"當月排班天數不可超過15天"
                });
                return null;
            }

            var holiday = await _holidayService.GetHolidayAsync(dto.WorkDate);

            if (holiday is not null)
            {
                _logger.Write(new Log
                {
                    Status = ApiResultStatus.Failed,
                    Message = "國定假日不可排班"
                });
            }

            var scheduleData = new Schedule
            {
                EmployeeId = dto.EmployeeId,
                WorkDate = dto.WorkDate,
            };



            _context.Schedules.Add(scheduleData);
            await _context.SaveChangesAsync();


        var employeeName =await  _context.Schedules
                                                            .Include(x => x.Employee)
                                                            .Where(x=>x.EmployeeId==dto.EmployeeId)
                                                            .Select(x => x.Employee.Name).FirstOrDefaultAsync();
            return new ScheduleDto
            {
                Id = scheduleData.Id,
                EmployeeId = scheduleData.EmployeeId,
                EmployeeName = employeeName,
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


        //員工當月上班次數

        public async Task<List<EmployeeReportDto>>  GetMonthlyWorkDays(int year, int month)
        {
            //當月資料
            var monthlyData = await _context.Schedules.
                Include(x => x.Employee)
                .Where(x => x.WorkDate.Year == year && x.WorkDate.Month == month)
                .GroupBy(x => new
                {
                    x.EmployeeId,
                    x.Employee.Name
                })
                .Select(g=>new 
                {
                    EmployeeId=g.Key.EmployeeId,  // 分組鍵
                    EmployeeName =g.Key.Name,   // 分組鍵
                    MonthlyWorkDays =g.Count()   //聚合結果
                })
                .ToListAsync();

             //年度資料
             var yearlyData= await _context.Schedules
                                                  . Include(x => x.Employee)
                                                  .Where(x=>x.WorkDate.Year==year)
                                                  .GroupBy(x=>x.EmployeeId)
                                                  .Select(g=>new {
                                                       EmployeeId=g.Key,
                                                      YearlyWorkDays = g.Count()
                                                  })
                                                   .ToDictionaryAsync(
                                                        x => x.EmployeeId,
                                                        x => x.YearlyWorkDays);

            return monthlyData.Select(x => new EmployeeReportDto
            {
                EmployeeId = x.EmployeeId,
                EmployeeName = x.EmployeeName,
                MonthlyWorkDays=x.MonthlyWorkDays,
                YearlyWorkDays= yearlyData.GetValueOrDefault(x.EmployeeId, 0) //沒資料預設給0
            })
                .OrderByDescending(x => x.MonthlyWorkDays)
            .ToList();
        }


        //只允許添加下個月的排班
        private bool IsNextMonth(DateOnly workDate)
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            var nextMonth = today.AddMonths(1);

            return workDate.Year == nextMonth.Year && workDate.Month == nextMonth.Month;
        }

        //是否為假日
        private bool IsWeekend(DateOnly workDate)
        {
            return workDate.DayOfWeek== DayOfWeek.Saturday || workDate.DayOfWeek== DayOfWeek.Sunday;
        }
    }
}
