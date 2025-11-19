using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApplication1Data.models;
using WebApplication1Data.Repositories;

namespace WebApplication1Data.Interfaces
{
    public interface IScheduleRepository : IRepository<ScheduleItem>
    {
        Task<IEnumerable<ScheduleItem>> GetScheduleForGroupAsync(string groupName);
        Task<IEnumerable<ScheduleItem>> GetScheduleForDayAsync(string groupName, DayOfWeek dayOfWeek);
        Task<IEnumerable<ScheduleItem>> GetScheduleForGroupWithDetailsAsync(string groupName);
        Task<IEnumerable<ScheduleItem>> GetScheduleForTeacherAsync(int teacherId);
    }
}