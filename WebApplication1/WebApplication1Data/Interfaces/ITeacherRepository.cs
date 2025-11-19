using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApplication1Data.models;
using WebApplication1Data.Repositories;

namespace WebApplication1Data.Interfaces
{
    public interface ITeacherRepository : IRepository<Teacher>
    {
        Task<Teacher?> GetByUserIdAsync(string userId);
        Task<IEnumerable<Teacher>> GetTeachersWithUserInfoAsync();
    }
}
