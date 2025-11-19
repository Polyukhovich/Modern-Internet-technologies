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
    public class TeacherRepository : Repository<Teacher>, ITeacherRepository
    {
        public TeacherRepository(dbContext context) : base(context) { }

        public async Task<Teacher?> GetByUserIdAsync(string userId)
        {
            return await _context.Set<Teacher>()
                .Include(t => t.User)
                .FirstOrDefaultAsync(t => t.UserId == userId);
        }

        public async Task<IEnumerable<Teacher>> GetTeachersWithUserInfoAsync()
        {
            return await _context.Set<Teacher>()
                .Include(t => t.User)
                .ToListAsync();
        }
    }
}
