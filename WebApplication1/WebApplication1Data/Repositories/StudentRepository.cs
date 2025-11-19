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
    public class StudentRepository : Repository<Student>, IStudentRepository
    {
        public StudentRepository(dbContext context) : base(context) { }

        public async Task<Student?> GetByUserIdAsync(string userId)
        {
            return await _context.Set<Student>()
                .Include(s => s.User)
                .FirstOrDefaultAsync(s => s.UserId == userId);
        }

        public async Task<IEnumerable<Student>> GetStudentsWithUserInfoAsync()
        {
            return await _context.Set<Student>()
                .Include(s => s.User)
                .ToListAsync();
        }
    }
}
