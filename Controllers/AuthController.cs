using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShiftPro.Dtos;
using ShiftPro.Interfaces;
using ShiftPro.Services.Auth;

namespace ShiftPro.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _service;
        public AuthController(IAuthService service)
        {
            _service = service;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var result = await _service.Login(loginDto);

            if (result == null)
            {
                return BadRequest("帳號或密碼錯誤");
            }
            return Ok(result);
        }
    }
}
