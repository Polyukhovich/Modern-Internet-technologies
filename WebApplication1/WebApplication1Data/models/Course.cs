using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApplication1Data.models
{
    public class Course
    {
        public int Id { get; set; }
        public string Title { get; set; } = default!;
        public string? Description { get; set; }

        // Викладач, який веде курс
        public int TeacherId { get; set; }
        public Teacher Teacher { get; set; } = default!;

        public ICollection<Grade>? Grades { get; set; }
        public ICollection<Material>? Materials { get; set; }
        public ICollection<ScheduleItem>? ScheduleItems { get; set; }
    }
}
