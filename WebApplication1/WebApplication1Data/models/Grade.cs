using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WebApplication1Data.models
{
    public class Grade
    {
        public int Id { get; set; }

        public int? CourseId { get; set; }
        public Course? Course { get; set; } = default!;

        public int? StudentId { get; set; }
        public Student? Student { get; set; } = default!;

        public decimal Value { get; set; }   // оцінка
        public DateTime DateSet { get; set; }

        // Викладач, який виставив
        public int? TeacherId { get; set; }
        public Teacher? Teacher { get; set; }

        // Додамо GroupName для зручності
        [NotMapped]
        public string GroupName { get; set; } = string.Empty;

    }
}
