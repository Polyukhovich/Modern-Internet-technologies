using Microsoft.EntityFrameworkCore;
using WebApplication1Data.Data;
using WebApplication1Data.Interfaces;
using WebApplication1Data.models;

namespace WebApplication1Data.Repositories
{
    public class ScheduleRepository : Repository<ScheduleItem>, IScheduleRepository
    {
        public ScheduleRepository(dbContext context) : base(context) { }

        public async Task<IEnumerable<ScheduleItem>> GetScheduleForGroupAsync(string groupName)
        {
            return await _context.Set<ScheduleItem>()
                .Where(s => s.GroupName == groupName)
                .OrderBy(s => s.DayOfWeek)
                .ThenBy(s => s.StartTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<ScheduleItem>> GetScheduleForDayAsync(string groupName, DayOfWeek dayOfWeek)
        {
            return await _context.Set<ScheduleItem>()
                .Where(s => s.GroupName == groupName && s.DayOfWeek == dayOfWeek)
                .OrderBy(s => s.StartTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<ScheduleItem>> GetScheduleForGroupWithDetailsAsync(string groupName)
        {
            return await _context.Set<ScheduleItem>()
                .Include(s => s.Course)
                .ThenInclude(c => c.Teacher)
                .ThenInclude(t => t.User)
                .Where(s => s.GroupName == groupName)
                .OrderBy(s => s.DayOfWeek)
                .ThenBy(s => s.StartTime)
                .ToListAsync();
        }
        public async Task<IEnumerable<ScheduleItem>> GetScheduleForTeacherAsync(int teacherId)
        {
            return await _context.Set<ScheduleItem>()
                .Include(s => s.Course)
                .ThenInclude(c => c.Teacher)
                .ThenInclude(t => t.User)
                .Where(s => s.Course.TeacherId == teacherId)
                .OrderBy(s => s.DayOfWeek)
                .ThenBy(s => s.StartTime)
                .ToListAsync();
        }
    }
}