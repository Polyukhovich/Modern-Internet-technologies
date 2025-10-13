using System;
using System.Collections.Generic;
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

        public DayOfWeek DayOfWeek { get; set; } // понеділок, вівторок тощо
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }

        public string Room { get; set; } = string.Empty;
        public string GroupName { get; set; } = string.Empty;
    }
}
