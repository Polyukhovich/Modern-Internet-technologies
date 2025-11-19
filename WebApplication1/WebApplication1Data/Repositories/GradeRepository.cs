using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApplication1Data.Interfaces;
using WebApplication1Data.models;
using WebApplication1Data.Data;

namespace WebApplication1Data.Repositories
{
    public class GradeRepository : Repository<Grade>, IGradeRepository
    {
        public GradeRepository(dbContext context) : base(context) { }

        public async Task<IEnumerable<Grade>> GetStudentGradesAsync(int studentId)
        {
            return await _context.Set<Grade>()
                .Include(g => g.Course)
                .Include(g => g.Teacher)
                .ThenInclude(t => t.User)
                .Where(g => g.StudentId == studentId)
                .OrderByDescending(g => g.DateSet)
                .ToListAsync();
        }

        public async Task<IEnumerable<Grade>> GetTeacherGradesAsync(int teacherId)
        {
            return await _context.Set<Grade>()
                .Include(g => g.Course)
                .Include(g => g.Student)
                .ThenInclude(s => s.User)
                .Where(g => g.Course.TeacherId == teacherId)
                .OrderByDescending(g => g.DateSet)
                .ToListAsync();
        }

        public async Task<IEnumerable<Grade>> GetCourseGradesAsync(int courseId)
        {
            return await _context.Set<Grade>()
                .Include(g => g.Student)
                .ThenInclude(s => s.User)
                .Include(g => g.Teacher)
                .ThenInclude(t => t.User)
                .Where(g => g.CourseId == courseId)
                .OrderByDescending(g => g.DateSet)
                .ToListAsync();
        }
        public async Task<Grade?> GetGradeWithDetailsAsync(int id)
        {
            return await _context.Set<Grade>()
                .Include(g => g.Course)
                .Include(g => g.Student)
                .ThenInclude(s => s.User)
                .Include(g => g.Teacher)
                .ThenInclude(t => t.User)
                .FirstOrDefaultAsync(g => g.Id == id);
        }
        public async Task<decimal> GetStudentAverageGradeAsync(int studentId)
        {
            return await _context.Set<Grade>()
                .Where(g => g.StudentId == studentId)
                .AverageAsync(g => g.Value);
        }

        public async Task<decimal> GetCourseAverageGradeAsync(int courseId)
        {
            return await _context.Set<Grade>()
                .Where(g => g.CourseId == courseId)
                .AverageAsync(g => g.Value);
        }

        public async Task<IEnumerable<Grade>> GetStudentGradesByCourseAsync(int studentId, int courseId)
        {
            return await _context.Set<Grade>()
                .Include(g => g.Course)
                .Include(g => g.Teacher)
                .ThenInclude(t => t.User)
                .Where(g => g.StudentId == studentId && g.CourseId == courseId)
                .OrderByDescending(g => g.DateSet)
                .ToListAsync();
        }
    }
}
