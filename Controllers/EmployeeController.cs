using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShiftPro.Dtos;
using ShiftPro.Interfaces;
using ShiftPro.Services.Employees;
using System.Security.Claims;

namespace ShiftPro.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeService _service;
        public EmployeeController(IEmployeeService service)
        {
            _service = service;
        }

        [Authorize(Roles = "Admin,Boss")]
        [HttpGet]
        public async Task<IActionResult> GetAllEmployees()
        {
            var result = await _service.GetAllEmployees();

            if (result == null)
            {
                return NotFound("查無Employees資料");
            }
            return Ok(result);
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetEmployeeById()
        {

            var user = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            var userId = int.Parse(user);

            var result =await  _service.GetEmployeeById(userId);

            if (result == null)
            {
                return NotFound("查無EmployeeId資料");
            }
            return Ok(result);
        }

        //[Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateEmployee( [FromBody] CreateEmployeeDto dto)
        {
            var result = await _service.CreateEmployee(dto);

            if (result == null)
            {
                return BadRequest("創建Employee失敗");
            }
            return Ok(new
            {
                Message = "創建此Employee成功"
            });
        }

        [Authorize]
        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateEmployee([FromRoute] int id, [FromBody] UpdateEmployeeDto dto)
        {

            var user = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            var userId = int.Parse(user);

            if (id != userId && !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            var result = await  _service.UpdateEmployee(id,dto);

            if (result == false)
            {
                return BadRequest("更新Employee失敗");
            }
            return Ok(new
            {
                Message = "更新此Employee成功"
            });
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee([FromRoute] int id)
        {
            var result =await _service.DeleteEmployee(id);

            if (result == false)
            {
                return BadRequest("刪除Employee失敗");
            }
            return Ok(new { 
                Message="刪除此Employee成功"
            });
        }
    }
}
