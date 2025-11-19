using Microsoft.EntityFrameworkCore;
using WebApplication1Data.Data;
using WebApplication1Data.Interfaces;
using WebApplication1Data.models;

namespace WebApplication1Data.Repositories
{
    public class MaterialRepository : Repository<Material>, IMaterialRepository
    {
        public MaterialRepository(dbContext context) : base(context) { }

        public async Task<IEnumerable<Material>> GetCourseMaterialsAsync(int courseId)
        {
            return await _context.Set<Material>()
                .Include(m => m.Course)
                .Include(m => m.UploadedBy)
                .Where(m => m.CourseId == courseId)
                .OrderByDescending(m => m.Id)
                .ToListAsync();
        }

        public async Task<IEnumerable<Material>> GetTeacherMaterialsAsync(int teacherId)
        {
            return await _context.Set<Material>()
                .Include(m => m.Course)
                .Include(m => m.UploadedBy)
                .Where(m => m.Course.TeacherId == teacherId)
                .OrderByDescending(m => m.Id)
                .ToListAsync();
        }

        public async Task<IEnumerable<Material>> GetRecentMaterialsAsync(int count)
        {
            return await _context.Set<Material>()
                .Include(m => m.Course)
                .Include(m => m.UploadedBy)
                .OrderByDescending(m => m.Id)
                .Take(count)
                .ToListAsync();
        }
    }
}