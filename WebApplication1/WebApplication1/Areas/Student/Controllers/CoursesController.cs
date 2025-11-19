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
    public class CoursesController : Controller
    {
        private readonly ICourseRepository _courseRepository;
        private readonly IStudentRepository _studentRepository;
        private readonly UserManager<User> _userManager;

        public CoursesController(
            ICourseRepository courseRepository,
            IStudentRepository studentRepository,
            UserManager<User> userManager)
        {
            _courseRepository = courseRepository;
            _studentRepository = studentRepository;
            _userManager = userManager;
        }

        // GET: Student/Courses
        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var student = await _studentRepository.GetByUserIdAsync(currentUser.Id);

            if (student == null) return NotFound();

            // Використовуємо репозиторій замість прямого доступу до контексту
            var courses = await _courseRepository.GetCoursesByGroupAsync(student.GroupName);

            ViewBag.Student = student;
            return View(courses);
        }

        // GET: Student/Courses/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var currentUser = await _userManager.GetUserAsync(User);
            var student = await _studentRepository.GetByUserIdAsync(currentUser.Id);

            if (student == null) return NotFound();

            // Використовуємо репозиторій для отримання курсу з деталями
            var course = await _courseRepository.GetCourseWithDetailsAsync(id.Value);

            // Додаткова перевірка, що курс належить групі студента
            if (course == null || course.GroupName != student.GroupName)
                return NotFound();

            ViewBag.Student = student;
            return View(course);
        }

        // GET: Student/Courses/Materials/5
        public async Task<IActionResult> Materials(int? id)
        {
            if (id == null) return NotFound();

            var currentUser = await _userManager.GetUserAsync(User);
            var student = await _studentRepository.GetByUserIdAsync(currentUser.Id);

            if (student == null) return NotFound();

            var course = await _courseRepository.GetCourseWithDetailsAsync(id.Value);

            // Перевірка доступу
            if (course == null || course.GroupName != student.GroupName)
                return NotFound();

            ViewBag.Course = course;
            ViewBag.Student = student;
            return View(course.Materials);
        }
    }
}