using Microsoft.EntityFrameworkCore;
using ShiftPro.Data;
using ShiftPro.Dtos;
using ShiftPro.Helpers;
using ShiftPro.Interfaces;
using ShiftPro.Services.JWT;

namespace ShiftPro.Services.Auth
{
    public class AuthService: IAuthService
    {
        private readonly AppDbContext _context;
        private readonly FileLogger _logger;
        private readonly JwtService _jwtService;

        public AuthService(AppDbContext context, FileLogger logger, JwtService jwtService)

        {
            _context = context;
            _logger = logger;
            _jwtService = jwtService;
        }


        public async Task<LoginResultDto> Login(LoginDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Account))
            {
                _logger.Write(new Log
                {
                    Status = ApiResultStatus.Failed,
                    Message = "Account填寫格式不正確",

                });
                return null;
            }

            if (string.IsNullOrWhiteSpace(dto.Password))
            {
                _logger.Write(new Log
                {
                    Status = ApiResultStatus.Failed,
                    Message = "Password填寫格式不正確",
                });
                return null;
            }

            var employee = await _context.Employees
                                                                .FirstOrDefaultAsync(x => x.Account == dto.Account && x.IsActived);

            if (employee == null)
            {
                _logger.Write(new Log
                {
                    Status = ApiResultStatus.Failed,
                    Message = "找不到此員工帳號",
                });
                return null;
            }

            // 驗證密碼
            var passwordVerity = BCrypt.Net.BCrypt.Verify(dto.Password, employee.PasswordHash);

            if (!passwordVerity)
            {
                _logger.Write(new Log
                {
                    Status = ApiResultStatus.Failed,
                    Message = "密碼驗證不通過",
                    Data = dto.Account.ToString()
                });
                return null;
            }


            //產生token
            var token = _jwtService.GenerateToken(employee);

            _logger.Write(new Log
            {
                Status = ApiResultStatus.Success,
                Message = employee.Account + "登入成功",
            });

            return new LoginResultDto
            {
                Name=employee.Name,
                Role=employee.Role.ToString(),
                Token=token,
            };
        }
    }
}
