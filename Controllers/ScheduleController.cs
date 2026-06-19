using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShiftPro.Dtos;
using ShiftPro.Interfaces;

namespace ShiftPro.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScheduleController : ControllerBase
    {
        private readonly IScheduleService _service;
        public ScheduleController(IScheduleService service)
        {
            _service = service;
        }

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

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAllSchedules([FromRoute] int id)
        {
            var result = await _service.GetSchedulesById(id);

            if (result == null)
            {
                return NotFound("找不到此Schedule");
            }
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateSchedule([FromBody] CreateScheduleDto dto)
        {
            var result =await _service.CreateSchedule(dto);

            if (result == null)
            {
                return BadRequest("請確認員工是否存在、當天人數是否已滿，或該員工本月是否已達 15 天");
            }
            return Ok(new {
               Message="創立Schedule成功",
                Result= result
            });
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateSchedule([FromRoute] int id, [FromBody] UpdateScheduleDto dto)
        {
            var result = await _service.UpdateSchedule(id, dto);

            if (result == false)
            {
                return BadRequest("更新Schedule失敗");
            }
            return Ok(new {
                Message = "更新Schedule成功"
                , Result= result
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSchedule([FromRoute] int id)
        {
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
