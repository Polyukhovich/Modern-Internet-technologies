using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApplication1Data.models;
using WebApplication1Data.Repositories;

namespace WebApplication1Data.Interfaces
{
    public interface IGradeRepository : IRepository<Grade>
    {
        Task<IEnumerable<Grade>> GetStudentGradesAsync(int studentId);
        Task<IEnumerable<Grade>> GetTeacherGradesAsync(int teacherId);
        Task<IEnumerable<Grade>> GetCourseGradesAsync(int courseId);
        Task<Grade?> GetGradeWithDetailsAsync(int id);

        Task<decimal> GetStudentAverageGradeAsync(int studentId);
        Task<decimal> GetCourseAverageGradeAsync(int courseId);
        Task<IEnumerable<Grade>> GetStudentGradesByCourseAsync(int studentId, int courseId);

    }
}
