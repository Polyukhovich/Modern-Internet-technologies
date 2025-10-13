using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace WebApplication1Data.Repositories
{
    // Базовий інтерфейс для універсальної роботи з будь-якими сутностями
    public interface IRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync(); // Отримати всі записи
        Task<T?> GetByIdAsync(int id); // Отримати запис за ідентифікатором
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate); // Знайти за умовою
        Task AddAsync(T entity); // Додати сутність
        void Update(T entity); // Оновити сутність
        void Delete(T entity); // Видалити сутність
        Task SaveChangesAsync(); // Зберегти зміни
        
        Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate); // Перевірка існування (завдання 2)
    }

}
