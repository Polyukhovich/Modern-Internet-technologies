using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApplication1Data.Interfaces;
using WebApplication1Data.Models;
using WebApplication1Data.models;

namespace WebApplication1.Controllers.Teacher
{
    [Area("Teacher")]
    [Authorize(Roles = "Teacher")]
    public class DashboardController : Controller
    {
        private readonly ITeacherRepository _teacherRepository;
        private readonly ICourseRepository _courseRepository;
        private readonly IGradeRepository _gradeRepository;
        private readonly UserManager<User> _userManager;

        public DashboardController(
            ITeacherRepository teacherRepository,
            ICourseRepository courseRepository,
            IGradeRepository gradeRepository,
            UserManager<User> userManager)
        {
            _teacherRepository = teacherRepository;
            _courseRepository = courseRepository;
            _gradeRepository = gradeRepository;
            _userManager = userManager;
        }

        // GET: Teacher/Dashboard
        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var teacher = await _teacherRepository.GetByUserIdAsync(currentUser.Id);

            if (teacher == null) return NotFound();

            // Статистика з використанням репозиторіїв
            var teacherCourses = await _courseRepository.GetTeacherCoursesAsync(teacher.Id);
            var teacherGrades = await _gradeRepository.GetTeacherGradesAsync(teacher.Id);

            var coursesCount = teacherCourses.Count();
            var gradesCount = teacherGrades.Count();

            // Унікальні студенти, яким виставлено оцінки
            var studentsCount = teacherGrades
                .Select(g => g.StudentId)
                .Distinct()
                .Count();

            var recentGrades = teacherGrades.Take(5).ToList();
            var myCourses = teacherCourses.Take(5).ToList();

            ViewBag.Teacher = teacher;
            ViewBag.CoursesCount = coursesCount;
            ViewBag.GradesCount = gradesCount;
            ViewBag.StudentsCount = studentsCount;
            ViewBag.RecentGrades = recentGrades;
            ViewBag.MyCourses = myCourses;

            return View();
        }
    }
}