using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApplication1Data.models;
using WebApplication1Data.Repositories;

namespace WebApplication1Data.Interfaces
{
    public interface ICourseRepository : IRepository<Course>
    {
        Task<IEnumerable<Course>> GetTeacherCoursesAsync(int teacherId);
        Task<IEnumerable<Course>> GetCoursesWithDetailsAsync();
        Task<Course?> GetCourseWithDetailsAsync(int id);

        Task<IEnumerable<Course>> GetCoursesByGroupAsync(string groupName);
        Task<IEnumerable<Course>> GetCoursesByTeacherAndGroupAsync(int teacherId, string groupName);

        // Для роботи з матеріалами через курс
        Task<IEnumerable<Material>> GetCourseMaterialsAsync(int courseId);
        Task<IEnumerable<ScheduleItem>> GetCourseScheduleAsync(int courseId);

        // Для роботи з розкладом
        Task<IEnumerable<ScheduleItem>> GetGroupScheduleAsync(string groupName);

    }
}
