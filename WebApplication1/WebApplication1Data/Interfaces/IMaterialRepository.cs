using WebApplication1Data.models;
using WebApplication1Data.Repositories;

namespace WebApplication1Data.Interfaces
{
    public interface IMaterialRepository : IRepository<Material>
    {
        Task<IEnumerable<Material>> GetCourseMaterialsAsync(int courseId);
        Task<IEnumerable<Material>> GetTeacherMaterialsAsync(int teacherId);
        Task<IEnumerable<Material>> GetRecentMaterialsAsync(int count);
    }
}