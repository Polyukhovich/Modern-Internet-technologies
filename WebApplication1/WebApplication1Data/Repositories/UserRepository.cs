using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApplication1Data.Data;
using WebApplication1Data.Interfaces;
using WebApplication1Data.Models;

namespace WebApplication1Data.Repositories
{

    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(dbContext context) : base(context) { }

        // Метод для пошуку користувача за Email
        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Set<User>().FirstOrDefaultAsync(u => u.Email == email);

        }
    }
}
