using ShiftPro.Dtos;

namespace ShiftPro.Interfaces
{
    public interface IHolidayService
    {
        Task<HolidayDto?> GetHolidayAsync(DateOnly date);

    }
}
