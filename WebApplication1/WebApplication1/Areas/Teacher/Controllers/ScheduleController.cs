using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApplication1Data.Interfaces;
using WebApplication1Data.models;
using WebApplication1Data.Models;

namespace WebApplication1.Controllers.Teacher
{
    [Area("Teacher")]
    [Authorize(Roles = "Teacher")]
    public class ScheduleController : Controller
    {
        private readonly IScheduleRepository _scheduleRepository;
        private readonly ITeacherRepository _teacherRepository;
        private readonly ICourseRepository _courseRepository;
        private readonly UserManager<User> _userManager;

        public ScheduleController(
            IScheduleRepository scheduleRepository,
            ITeacherRepository teacherRepository,
            ICourseRepository courseRepository,
            UserManager<User> userManager)
        {
            _scheduleRepository = scheduleRepository;
            _teacherRepository = teacherRepository;
            _courseRepository = courseRepository;
            _userManager = userManager;
        }

        // GET: Teacher/Schedule
        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var teacher = await _teacherRepository.GetByUserIdAsync(currentUser.Id);

            if (teacher == null)
            {
                return NotFound();
            }

            // Отримуємо всі курси викладача
            var courses = await _courseRepository.GetTeacherCoursesAsync(teacher.Id);
            var courseIds = courses.Select(c => c.Id).ToList();

            // Отримуємо розклад для курсів цього викладача
            var allScheduleItems = await _scheduleRepository.GetScheduleForGroupWithDetailsAsync("");
            var scheduleItems = await _scheduleRepository.GetScheduleForTeacherAsync(teacher.Id);

            // Групуємо по дням тижня для зручного відображення
            var scheduleByDay = scheduleItems
                .GroupBy(s => s.DayOfWeek)
                .ToDictionary(g => g.Key, g => g.ToList());

            ViewBag.Teacher = teacher;
            ViewBag.ScheduleByDay = scheduleByDay;
            ViewBag.CurrentWeek = GetCurrentWeekDates();

            return View();
        }

        // GET: Teacher/Schedule/Week?date=2024-01-15
        public async Task<IActionResult> Week(DateTime? date)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var teacher = await _teacherRepository.GetByUserIdAsync(currentUser.Id);

            if (teacher == null)
            {
                return NotFound();
            }

            var targetDate = date ?? DateTime.Today;
            var weekDates = GetWeekDates(targetDate);

            // Отримуємо всі курси викладача
            var courses = await _courseRepository.GetTeacherCoursesAsync(teacher.Id);
            var courseIds = courses.Select(c => c.Id).ToList();

            // Отримуємо розклад
            var allScheduleItems = await _scheduleRepository.GetScheduleForGroupWithDetailsAsync("");
            var scheduleItems = await _scheduleRepository.GetScheduleForTeacherAsync(teacher.Id);

            var weeklySchedule = new List<object>();

            foreach (var day in weekDates)
            {
                var daySchedule = scheduleItems
                    .Where(s => s.DayOfWeek == day.DayOfWeek)
                    .OrderBy(s => s.StartTime)
                    .Select(s => new
                    {
                        Id = s.Id,
                        CourseTitle = s.Course?.Title ?? "Невідомий курс",
                        StartTime = s.StartTime,
                        EndTime = s.EndTime,
                        Room = s.Room,
                        GroupName = s.GroupName,
                        ClassType = s.ClassType
                    })
                    .ToList();

                weeklySchedule.Add(new
                {
                    Date = day,
                    ScheduleItems = daySchedule
                });
            }

            ViewBag.WeeklySchedule = weeklySchedule;
            ViewBag.Teacher = teacher;
            ViewBag.WeekStart = weekDates.First();
            ViewBag.WeekEnd = weekDates.Last();

            return View("Weekly");
        }

        private Dictionary<DayOfWeek, DateTime> GetCurrentWeekDates()
        {
            var today = DateTime.Today;
            var startOfWeek = today.AddDays(-(int)today.DayOfWeek + (int)DayOfWeek.Monday);

            var weekDates = new Dictionary<DayOfWeek, DateTime>();
            for (int i = 0; i < 7; i++)
            {
                weekDates[(DayOfWeek)((int)DayOfWeek.Monday + i)] = startOfWeek.AddDays(i);
            }

            return weekDates;
        }

        private List<DateTime> GetWeekDates(DateTime date)
        {
            var startOfWeek = date.AddDays(-(int)date.DayOfWeek + (int)DayOfWeek.Monday);
            var weekDates = new List<DateTime>();

            for (int i = 0; i < 7; i++)
            {
                weekDates.Add(startOfWeek.AddDays(i));
            }

            return weekDates;
        }
    }
}