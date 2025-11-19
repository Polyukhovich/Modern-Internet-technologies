using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApplication1Data.models
{
    public class ScheduleItem
    {
        public int Id { get; set; }

        public int CourseId { get; set; }
        public Course Course { get; set; } = default!;

        public DayOfWeek DayOfWeek { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }

        public string Room { get; set; } = string.Empty;
        public string GroupName { get; set; } = string.Empty;

        // Додамо тип заняття для більшої інформативності
        public ClassType ClassType { get; set; } = ClassType.Lecture;
    }

    public enum ClassType
    {
        [Display(Name = "Лекція")]
        Lecture,
        [Display(Name = "Практика")]
        Practice,
        [Display(Name = "Лабораторна")]
        Lab,
        [Display(Name = "Семінар")]
        Seminar,
        [Display(Name = "Консультація")]
        Consultation
    }
}