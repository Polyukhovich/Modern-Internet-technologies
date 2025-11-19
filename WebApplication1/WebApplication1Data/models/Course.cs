using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WebApplication1Data.models
{
    public class Course
    {
        public int Id { get; set; }
        public string? Title { get; set; } = default!;
        public string? Description { get; set; }

        // Викладач, який веде курс
        public int TeacherId { get; set; }
        public Teacher? Teacher { get; set; }

        // Назва групи, для якої призначений курс
        [Required(ErrorMessage = "Оберіть групу")]
        public string GroupName { get; set; } = string.Empty;


        public ICollection<Grade>? Grades { get; set; }
        public ICollection<Material>? Materials { get; set; }
        public ICollection<ScheduleItem>? ScheduleItems { get; set; }
    }
}
