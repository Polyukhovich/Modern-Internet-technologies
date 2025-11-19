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
    public class GradesController : Controller
    {
        private readonly IGradeRepository _gradeRepository;
        private readonly IStudentRepository _studentRepository;
        private readonly ICourseRepository _courseRepository;
        private readonly UserManager<User> _userManager;

        public GradesController(
            IGradeRepository gradeRepository,
            IStudentRepository studentRepository,
            ICourseRepository courseRepository,
            UserManager<User> userManager)
        {
            _gradeRepository = gradeRepository;
            _studentRepository = studentRepository;
            _courseRepository = courseRepository;
            _userManager = userManager;
        }

        // GET: Student/Grades
        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var student = await _studentRepository.GetByUserIdAsync(currentUser.Id);

            if (student == null) return NotFound();

            // Отримуємо оцінки студента через репозиторій
            var grades = await _gradeRepository.GetStudentGradesAsync(student.Id);

            // Розраховуємо середній бал
            var averageGrade = grades.Any() ? grades.Average(g => g.Value) : 0;

            ViewBag.Student = student;
            ViewBag.AverageGrade = Math.Round(averageGrade, 2);
            ViewBag.TotalGrades = grades.Count();

            return View(grades);
        }

        // GET: Student/Grades/ByCourse
        public async Task<IActionResult> ByCourse()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var student = await _studentRepository.GetByUserIdAsync(currentUser.Id);

            if (student == null) return NotFound();

            // Отримуємо оцінки з группуванням по курсах
            var grades = await _gradeRepository.GetStudentGradesAsync(student.Id);
            var courses = await _courseRepository.GetCoursesByGroupAsync(student.GroupName);

            // Групуємо вручну, оскільки репозиторій повертає готовий список
            var gradesByCourse = courses.Select(course => new StudentGradesByCourseViewModel
            {
                Course = course,
                Grades = grades.Where(g => g.CourseId == course.Id).ToList(),
                AverageGrade = grades.Where(g => g.CourseId == course.Id).Any()
                    ? grades.Where(g => g.CourseId == course.Id).Average(g => g.Value)
                    : 0
            })
            .Where(vm => vm.Grades.Any()) // Показуємо тільки курси з оцінками
            .ToList();

            ViewBag.Student = student;
            return View(gradesByCourse);
        }

        // GET: Student/Grades/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var currentUser = await _userManager.GetUserAsync(User);
            var student = await _studentRepository.GetByUserIdAsync(currentUser.Id);

            if (student == null) return NotFound();

            // Використовуємо репозиторій для отримання оцінки з деталями
            var grade = await _gradeRepository.GetGradeWithDetailsAsync(id.Value);

            // Перевіряємо, що оцінка належить студенту
            if (grade == null || grade.StudentId != student.Id)
                return NotFound();

            ViewBag.Student = student;
            return View(grade);
        }
    }

    // ViewModel для группування оцінок по курсах
    public class StudentGradesByCourseViewModel
    {
        public Course Course { get; set; } = default!;
        public List<Grade> Grades { get; set; } = new();
        public decimal AverageGrade { get; set; }
    }
}