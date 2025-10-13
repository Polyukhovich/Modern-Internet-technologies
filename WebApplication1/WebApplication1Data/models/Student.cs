using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApplication1Data.Models;

namespace WebApplication1Data.models
{
    public class Student
    {
        public int Id { get; set; }

        // Зв'язок з User
        public string UserId { get; set; } = default!;
        public User User { get; set; } = default!;

        public string GroupName { get; set; } = string.Empty;
        public int YearOfStudy { get; set; }

        public ICollection<Grade>? Grades { get; set; }
    }
}
