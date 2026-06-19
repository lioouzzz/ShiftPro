using ShiftPro.Dtos;
using ShiftPro.Interfaces;

namespace ShiftPro.Services.Holidays
{
    public class HolidayService : IHolidayService
    {

        public Task<HolidayDto?> GetHolidayAsync(DateOnly date)
        {
            return Task.FromResult<HolidayDto?>(null);
        }
    }
}
