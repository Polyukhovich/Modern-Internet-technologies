using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApplication1Data.Interfaces;
using WebApplication1Data.Models;
using WebApplication1Data.models;

namespace WebApplication1.Controllers.Student
{
    [Area("Student")]
    [Authorize(Roles = "Student")]
    public class DashboardController : Controller
    {
        private readonly IStudentRepository _studentRepository;
        private readonly ICourseRepository _courseRepository;
        private readonly IGradeRepository _gradeRepository;
        private readonly UserManager<User> _userManager;

        public DashboardController(
            IStudentRepository studentRepository,
            ICourseRepository courseRepository,
            IGradeRepository gradeRepository,
            UserManager<User> userManager)
        {
            _studentRepository = studentRepository;
            _courseRepository = courseRepository;
            _gradeRepository = gradeRepository;
            _userManager = userManager;
        }

        // GET: Student/Dashboard
        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var student = await _studentRepository.GetByUserIdAsync(currentUser.Id);

            if (student == null) return NotFound();

            // Статистика з використанням репозиторіїв
            var courses = await _courseRepository.GetCoursesByGroupAsync(student.GroupName);
            var studentGrades = await _gradeRepository.GetStudentGradesAsync(student.Id);

            // ВИПРАВЛЕНО: Викликаємо методи Count()
            var totalCourses = courses.Count();
            var totalGrades = studentGrades.Count();
            var averageGrade = studentGrades.Any() ? Math.Round(studentGrades.Average(g => g.Value), 2) : 0;

            // Для розкладу потрібно буде додати метод в репозиторій
            var todaySchedule = await GetTodayScheduleAsync(student.GroupName, DateTime.Today.DayOfWeek);
            var recentGrades = studentGrades.Take(5).ToList();

            ViewBag.Student = student;
            ViewBag.TotalCourses = totalCourses;
            ViewBag.TotalGrades = totalGrades;
            ViewBag.AverageGrade = averageGrade;
            ViewBag.TodaySchedule = todaySchedule;
            ViewBag.RecentGrades = recentGrades;

            return View();
        }

        // GET: Student/Dashboard/Profile
        public async Task<IActionResult> Profile()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var student = await _studentRepository.GetByUserIdAsync(currentUser.Id);

            if (student == null) return NotFound();

            var courses = await _courseRepository.GetCoursesByGroupAsync(student.GroupName);
            var studentGrades = await _gradeRepository.GetStudentGradesAsync(student.Id);

            //  Викликаємо методи Count()
            ViewBag.Student = student;
            ViewBag.Courses = courses;
            ViewBag.TotalGrades = studentGrades.Count();
            ViewBag.AverageGrade = studentGrades.Any() ? Math.Round(studentGrades.Average(g => g.Value), 2) : 0;

            return View();
        }

        // Допоміжний метод для отримання розкладу на сьогодні
        private async Task<List<ScheduleItem>> GetTodayScheduleAsync(string groupName, DayOfWeek dayOfWeek)
        {
            // Поки що використовуємо контекст, але можна додати метод в репозиторій
            // TODO: Додати IScheduleRepository з методом GetScheduleForDayAsync
            return new List<ScheduleItem>(); // Заглушка
        }
    }
}