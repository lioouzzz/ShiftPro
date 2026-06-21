using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShiftPro.Data;
using ShiftPro.Dtos;
using ShiftPro.Interfaces;
using System.Security.Claims;

namespace ShiftPro.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScheduleController : ControllerBase
    {
        private readonly IScheduleService _service;
        private readonly AppDbContext _context;
        public ScheduleController(IScheduleService service, AppDbContext context)
        {
            _service = service;
            _context = context;
        }

        [Authorize(Roles = "Admin,Boss")]
        [HttpGet]
        public async Task<IActionResult> GetAllSchedules()
        {
            var result = await _service.GetAllSchedules();

            if (result == null)
            {
                return NotFound("找不到此Schedule");
            }
            return Ok(result);  
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetAllScheduleById()
        {
            var user = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            var userId = int.Parse(user);

            var result = await _service.GetSchedulesById(userId);

            if (result == null)
            {
                return NotFound("尚無排班紀錄");
            }
            return Ok(result);
        }

        [Authorize(Roles = "Employee,Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateSchedule([FromBody] CreateScheduleDto dto)
        {
            var user = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            var userId = int.Parse(user);

            if (dto.EmployeeId != userId && !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            try
            {
                var result = await _service.CreateSchedule(dto);

                if (result == null)
                {
                    return BadRequest("請確認員工是否存在、假日例假日不可排班、當天人數是否已滿，或該員工本月是否已達 15 天");

                }
                return Ok(new
                {
                    Message = "建立 Schedule 成功",
                    Result = result
                });

            }
            catch (Exception ex) {
                return BadRequest(new
                {
                    Message = ex.Message,
                });
            }
        }

        [Authorize(Roles = "Employee,Admin")]
        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateSchedule([FromRoute] int id, [FromBody] UpdateScheduleDto dto)
        {
            var user = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            var userId = int.Parse(user);


           var schedule =await  _context.Schedules.FirstOrDefaultAsync(x => x.Id == id);

            if (schedule == null)
            {
                return NotFound("查無Schedule資料");
            }

            if (schedule.EmployeeId != userId && !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            var result = await _service.UpdateSchedule(id, dto);

            if (result == false)
            {
                return BadRequest("請確認員工是否存在、假日例假日不可排班、當天人數是否已滿，或該員工本月是否已達 15 天");
            }
            return Ok(new {
                Message = "更新Schedule成功"
                , Result= result
            });
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSchedule([FromRoute] int id)
        {
            var user = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            var userId = int.Parse(user);

            var schedule = await _context.Schedules.FirstOrDefaultAsync(x => x.Id == id);

            if (schedule == null)
            {
                return NotFound("查無Schedule資料");
            }

            if (schedule.EmployeeId != userId && !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            var result =await _service.DeleteSchedule(id);

            if (result == false)
            {
                return BadRequest("刪除Schedule失敗");
            }
            return Ok(new {
                Message="刪除Schedule成功",
                Result= result
            });
        }

        [Authorize(Roles = "Boss,Admin")]
        [HttpGet("Monthly")]
        public async Task<IActionResult> GetMonthlySchedules([FromQuery] int year, [FromQuery] int month)
        {
            var result = await _service.GetMonthlySchedules(year, month);

            if (result == null)
            {

                return BadRequest("查詢月份排班失敗");
            }
                return Ok( result );
            }

        [Authorize(Roles = "Boss,Admin")]
        [HttpGet("EmployeeReport")]
        public async Task<IActionResult> GetEmployeesMonthlyCount([FromQuery] int year, [FromQuery] int month)
        {
            var result  = await _service.GetMonthlyWorkDays(year, month);

            if (result == null)
            {
                return BadRequest("無法取得當月員工排班天數");
            }
            return Ok( result );
        }
    }
}
