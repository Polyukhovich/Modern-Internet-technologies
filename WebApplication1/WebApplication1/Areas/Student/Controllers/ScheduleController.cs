using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApplication1Data.Interfaces;
using WebApplication1Data.models;
using WebApplication1Data.Models;

namespace WebApplication1.Controllers.Student
{
    [Area("Student")]
    [Authorize(Roles = "Student")]
    public class ScheduleController : Controller
    {
        private readonly IScheduleRepository _scheduleRepository;
        private readonly IStudentRepository _studentRepository;
        private readonly UserManager<User> _userManager;

        public ScheduleController(
            IScheduleRepository scheduleRepository,
            IStudentRepository studentRepository,
            UserManager<User> userManager)
        {
            _scheduleRepository = scheduleRepository;
            _studentRepository = studentRepository;
            _userManager = userManager;
        }

        // GET: Student/Schedule
        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var student = await _studentRepository.GetByUserIdAsync(currentUser.Id);

            if (student == null) return NotFound();

            // Отримуємо розклад через репозиторій
            var scheduleItems = await _scheduleRepository.GetScheduleForGroupWithDetailsAsync(student.GroupName);

            // Групуємо по дням тижня
            var scheduleByDay = scheduleItems
                .GroupBy(s => s.DayOfWeek)
                .ToDictionary(g => g.Key, g => g.ToList());

            ViewBag.Student = student;
            ViewBag.ScheduleByDay = scheduleByDay;
            ViewBag.CurrentWeek = GetCurrentWeekDates();

            return View();
        }

        // GET: Student/Schedule/Week?date=2024-01-15
        public async Task<IActionResult> Week(DateTime? date)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var student = await _studentRepository.GetByUserIdAsync(currentUser.Id);

            if (student == null) return NotFound();

            var targetDate = date ?? DateTime.Today;
            var weekDates = GetWeekDates(targetDate);

            // Отримуємо розклад через репозиторій
            var scheduleItems = await _scheduleRepository.GetScheduleForGroupWithDetailsAsync(student.GroupName);

            var weeklySchedule = new List<object>();

            foreach (var day in weekDates)
            {
                var daySchedule = scheduleItems
                    .Where(s => s.DayOfWeek == day.DayOfWeek)
                    .OrderBy(s => s.StartTime)
                    .Select(s => new
                    {
                        Id = s.Id,
                        CourseTitle = s.Course.Title,
                        TeacherName = $"{s.Course.Teacher.User.LastName} {s.Course.Teacher.User.FirstName}",
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

            ViewBag.Student = student;
            ViewBag.WeeklySchedule = weeklySchedule;
            ViewBag.WeekStart = weekDates.First();
            ViewBag.WeekEnd = weekDates.Last();

            return View("Weekly");
        }

        // GET: Student/Schedule/Today
        public async Task<IActionResult> Today()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var student = await _studentRepository.GetByUserIdAsync(currentUser.Id);

            if (student == null) return NotFound();

            var today = DateTime.Today;
            var dayOfWeek = today.DayOfWeek;

            // Отримуємо розклад на сьогодні через репозиторій
            var todaySchedule = await _scheduleRepository.GetScheduleForGroupWithDetailsAsync(student.GroupName);
            todaySchedule = todaySchedule
                .Where(s => s.DayOfWeek == dayOfWeek)
                .OrderBy(s => s.StartTime)
                .ToList();

            ViewBag.Student = student;
            ViewBag.Today = today;
            ViewBag.TodaySchedule = todaySchedule;

            return View();
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