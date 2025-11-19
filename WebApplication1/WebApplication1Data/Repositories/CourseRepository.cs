using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApplication1Data.Data;
using WebApplication1Data.Interfaces;
using WebApplication1Data.models;

namespace WebApplication1Data.Repositories
{
    public class CourseRepository : Repository<Course>, ICourseRepository
    {
        public CourseRepository(dbContext context) : base(context) { }

        public async Task<IEnumerable<Course>> GetTeacherCoursesAsync(int teacherId)
        {
            return await _context.Set<Course>()
                .Include(c => c.Teacher)
                .ThenInclude(t => t.User)
                .Where(c => c.TeacherId == teacherId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Course>> GetCoursesWithDetailsAsync()
        {
            return await _context.Set<Course>()
                .Include(c => c.Teacher)
                .ThenInclude(t => t.User)
                .Include(c => c.Materials)
                .Include(c => c.ScheduleItems)
                .ToListAsync();
        }

        public async Task<Course?> GetCourseWithDetailsAsync(int id)
        {
            return await _context.Set<Course>()
                .Include(c => c.Teacher)
                .ThenInclude(t => t.User)
                .Include(c => c.Materials)
                .ThenInclude(m => m.UploadedBy)
                .Include(c => c.ScheduleItems)
                .FirstOrDefaultAsync(c => c.Id == id);
        }
        public async Task<IEnumerable<Course>> GetCoursesByGroupAsync(string groupName)
        {
            return await _context.Set<Course>()
                .Include(c => c.Teacher)
                .ThenInclude(t => t.User)
                .Where(c => c.GroupName == groupName)
                .ToListAsync();
        }

        public async Task<IEnumerable<Course>> GetCoursesByTeacherAndGroupAsync(int teacherId, string groupName)
        {
            return await _context.Set<Course>()
                .Include(c => c.Teacher)
                .ThenInclude(t => t.User)
                .Where(c => c.TeacherId == teacherId && c.GroupName == groupName)
                .ToListAsync();
        }
        public async Task<IEnumerable<Material>> GetCourseMaterialsAsync(int courseId)
        {
            return await _context.Set<Material>()
                .Include(m => m.UploadedBy)
                .Where(m => m.CourseId == courseId)
                .ToListAsync();
        }

        public async Task<IEnumerable<ScheduleItem>> GetCourseScheduleAsync(int courseId)
        {
            return await _context.Set<ScheduleItem>()
                .Where(s => s.CourseId == courseId)
                .OrderBy(s => s.DayOfWeek)
                .ThenBy(s => s.StartTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<ScheduleItem>> GetGroupScheduleAsync(string groupName)
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

    }
}
