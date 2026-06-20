using ShiftPro.Dtos;
using ShiftPro.Helpers;
using ShiftPro.Interfaces;
using System.Text.Json;

namespace ShiftPro.Services.Holidays
{
    public class HolidayService : IHolidayService
    {
        private readonly IWebHostEnvironment _env;
        private readonly FileLogger _logger;

        public HolidayService(IWebHostEnvironment env, FileLogger logger)
        {
            _env = env;
            _logger= logger;
        }
        public async  Task<HolidayDto?> GetHolidayAsync(DateOnly date)
        {
            var path = Path.Combine(_env.ContentRootPath, "Resources", "115年度國家辦公日曆.json");

            if (!File.Exists(path))
            {
                _logger.Write(new Log {
                    Status=ApiResultStatus.Error,
                    Message= "未讀取到115年度國家辦公日曆.json檔案"
                });
                return null;
            }

            var json = await File.ReadAllTextAsync(path);

            var holidays = JsonSerializer.Deserialize<List<HolidayDto>>(json);

            var holiday = holidays?
                                        .Where(x => !string.IsNullOrWhiteSpace(x.StartDate))
                                        .FirstOrDefault(x =>
                                            DateOnly.TryParse(x.StartDate, out var holidayDate) &&holidayDate == date);


            return holiday;
        }
    }
}
