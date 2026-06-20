using ShiftPro.Dtos;

namespace ShiftPro.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResultDto> Login(LoginDto dto);
    }
}
