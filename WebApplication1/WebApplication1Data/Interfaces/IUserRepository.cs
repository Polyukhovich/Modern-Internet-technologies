using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApplication1Data.Models;
using WebApplication1Data.Repositories;

namespace WebApplication1Data.Interfaces
{
    public interface IUserRepository : IRepository<User>
    {
        // Метод для пошуку користувача за унікальною властивістю (email)
        Task<User?> GetByEmailAsync(string email);
    }
}
