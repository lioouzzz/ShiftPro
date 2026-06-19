using ShiftPro.Dtos;

namespace ShiftPro.Interfaces
{
    public interface IScheduleService
    {
        Task<List<ScheduleDto>> GetAllSchedules();
        Task<ScheduleDto?> GetSchedulesById(int id);
        Task<ScheduleDto> CreateSchedule(CreateScheduleDto dto);
        Task<bool> UpdateSchedule(int id, UpdateScheduleDto dto);
        Task<bool> DeleteSchedule(int id);
    }
}
