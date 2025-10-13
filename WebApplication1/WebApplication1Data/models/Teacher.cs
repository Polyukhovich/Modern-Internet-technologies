using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApplication1Data.Models;

namespace WebApplication1Data.models
{
    public class Teacher
    {
        public int Id { get; set; }

        public string ApplicationUserId { get; set; } = default!;
        public User User { get; set; } = default!;

        public string Department { get; set; } = string.Empty;
        public ICollection<Course>? Courses { get; set; }
    }
}
