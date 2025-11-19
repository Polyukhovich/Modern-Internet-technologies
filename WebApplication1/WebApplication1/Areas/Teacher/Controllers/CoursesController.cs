using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebApplication1Data.Interfaces;
using WebApplication1Data.models;
using WebApplication1Data.Models;

// Додаємо псевдонім для уникнення конфлікту
using TeacherEntity = WebApplication1Data.models.Teacher;

namespace WebApplication1.Areas.Teacher.Controllers
{
    [Area("Teacher")]
    [Authorize(Roles = "Teacher")]
    public class CoursesController : Controller
    {
        private readonly ICourseRepository _courseRepository;
        private readonly ITeacherRepository _teacherRepository;
        private readonly IStudentRepository _studentRepository;
        private readonly IGradeRepository _gradeRepository;
        private readonly IMaterialRepository _materialRepository;
        private readonly IScheduleRepository _scheduleRepository;
        private readonly UserManager<User> _userManager;

        public CoursesController(
            ICourseRepository courseRepository,
            ITeacherRepository teacherRepository,
            IStudentRepository studentRepository,
            IGradeRepository gradeRepository,
            IMaterialRepository materialRepository,
            IScheduleRepository scheduleRepository,
            UserManager<User> userManager)
        {
            _courseRepository = courseRepository;
            _teacherRepository = teacherRepository;
            _studentRepository = studentRepository;
            _gradeRepository = gradeRepository;
            _materialRepository = materialRepository;
            _scheduleRepository = scheduleRepository;
            _userManager = userManager;
        }

        private async Task LoadGroupListAsync()
        {
            try
            {
                // Отримуємо список груп через репозиторій студентів
                var groups = await _studentRepository.GetStudentsWithUserInfoAsync();
                var distinctGroups = groups
                    .Where(s => !string.IsNullOrEmpty(s.GroupName))
                    .Select(s => s.GroupName)
                    .Distinct()
                    .OrderBy(g => g)
                    .ToList();

                ViewBag.GroupList = distinctGroups.Select(g => new SelectListItem { Value = g, Text = g }).ToList();
            }
            catch (Exception ex)
            {
                TempData["LoadGroupError"] = ex.GetBaseException()?.Message ?? ex.Message;
                ViewBag.GroupList = new List<SelectListItem>();
            }
        }

        // Отримання поточного викладача за UserId - ВИПРАВЛЕНО
        private async Task<TeacherEntity?> GetCurrentTeacherAsync()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
                return null;

            return await _teacherRepository.GetByUserIdAsync(currentUser.Id);
        }

        // GET: Teacher/Courses
        public async Task<IActionResult> Index()
        {
            var teacher = await GetCurrentTeacherAsync();
            if (teacher == null)
                return Forbid();

            // Використовуємо репозиторій замість прямого доступу
            var courses = await _courseRepository.GetTeacherCoursesAsync(teacher.Id);
            return View(courses);
        }

        // GET: Teacher/Courses/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var teacher = await GetCurrentTeacherAsync();
            if (teacher == null)
                return Forbid();

            // Використовуємо репозиторій для отримання курсу з деталями
            var course = await _courseRepository.GetCourseWithDetailsAsync(id.Value);

            // Перевіряємо, що курс належить викладачу - ВИПРАВЛЕНО
            if (course == null || course.TeacherId != teacher.Id)
                return NotFound();

            return View(course);
        }

        // GET: Teacher/Courses/Create
        public async Task<IActionResult> Create()
        {
            var teacher = await GetCurrentTeacherAsync();
            if (teacher == null)
                return Forbid();

            await LoadGroupListAsync();
            return View();
        }

        // POST: Teacher/Courses/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Description,GroupName")] Course course)
        {
            var teacher = await GetCurrentTeacherAsync();
            if (teacher == null)
                return Forbid();

            if (ModelState.IsValid)
            {
                try
                {
                    course.TeacherId = teacher.Id;
                    await _courseRepository.AddAsync(course);
                    await _courseRepository.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Курс успішно створено.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Помилка при збереженні: {ex.GetBaseException()?.Message ?? ex.Message}");
                }
            }

            await LoadGroupListAsync();
            return View(course);
        }

        // GET: Teacher/Courses/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var teacher = await GetCurrentTeacherAsync();
            if (teacher == null)
                return Forbid();

            var course = await _courseRepository.GetByIdAsync(id.Value);

            // Перевіряємо, що курс належить викладачу - ВИПРАВЛЕНО
            if (course == null || course.TeacherId != teacher.Id)
                return NotFound();

            await LoadGroupListAsync();
            return View(course);
        }

        // POST: Teacher/Courses/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,GroupName")] Course input)
        {
            if (id != input.Id)
                return NotFound();

            var teacher = await GetCurrentTeacherAsync();
            if (teacher == null)
                return Forbid();

            // Отримуємо курс з бази через репозиторій
            var course = await _courseRepository.GetByIdAsync(id);

            // Перевіряємо, що курс належить викладачу - ВИПРАВЛЕНО
            if (course == null || course.TeacherId != teacher.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    // Оновлюємо тільки поля, які можна редагувати
                    course.Title = input.Title;
                    course.Description = input.Description;
                    course.GroupName = input.GroupName;

                    await _courseRepository.UpdateAsync(course);
                    await _courseRepository.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Зміни збережено.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Помилка при збереженні: {ex.Message}");
                }
            }

            await LoadGroupListAsync();
            return View(course);
        }

        // GET: Teacher/Courses/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                TempData["ErrorMessage"] = "ID курсу не вказано";
                return RedirectToAction(nameof(Index));
            }

            var teacher = await GetCurrentTeacherAsync();
            if (teacher == null)
            {
                TempData["ErrorMessage"] = "Викладач не знайдений";
                return RedirectToAction(nameof(Index));
            }

            var course = await _courseRepository.GetByIdAsync(id.Value);

            // ВИПРАВЛЕНО: перевірка на null та TeacherId
            if (course == null || course.TeacherId != teacher.Id)
            {
                TempData["ErrorMessage"] = "Курс не знайдено або він не належить вам";
                return RedirectToAction(nameof(Index));
            }

            // Перевіряємо пов'язані дані через репозиторії
            var hasGrades = await _gradeRepository.ExistsAsync(g => g.CourseId == id);
            var hasMaterials = await _materialRepository.ExistsAsync(m => m.CourseId == id);
            var hasScheduleItems = await _scheduleRepository.ExistsAsync(s => s.CourseId == id);

            ViewBag.HasRelatedData = hasGrades || hasMaterials || hasScheduleItems;

            return View(course);
        }

        // POST: Teacher/Courses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var teacher = await GetCurrentTeacherAsync();
                if (teacher == null)
                {
                    TempData["ErrorMessage"] = "Викладач не знайдений. Увійдіть знову.";
                    return RedirectToAction(nameof(Index));
                }

                var course = await _courseRepository.GetByIdAsync(id);

                // ВИПРАВЛЕНО: перевірка на null та TeacherId
                if (course == null || course.TeacherId != teacher.Id)
                {
                    TempData["ErrorMessage"] = "Курс не знайдено або він не належить вам";
                    return RedirectToAction(nameof(Index));
                }

                // Перевіряємо наявність пов'язаних даних через репозиторії
                var hasGrades = await _gradeRepository.ExistsAsync(g => g.CourseId == id);
                var hasMaterials = await _materialRepository.ExistsAsync(m => m.CourseId == id);
                var hasScheduleItems = await _scheduleRepository.ExistsAsync(s => s.CourseId == id);

                if (hasGrades || hasMaterials || hasScheduleItems)
                {
                    TempData["ErrorMessage"] = "Неможливо видалити курс, оскільки він має пов'язані дані. Спочатку видаліть: ";
                    if (hasGrades) TempData["ErrorMessage"] += "оцінки, ";
                    if (hasMaterials) TempData["ErrorMessage"] += "матеріали, ";
                    if (hasScheduleItems) TempData["ErrorMessage"] += "заняття в розкладі";

                    return RedirectToAction(nameof(Delete), new { id });
                }

                // Видаляємо курс через репозиторій
                _courseRepository.Delete(course);
                await _courseRepository.SaveChangesAsync();

                TempData["SuccessMessage"] = $"Курс '{course.Title}' успішно видалено.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Помилка при видаленні: {ex.Message}";
                return RedirectToAction(nameof(Delete), new { id });
            }
        }

        private bool CourseExists(int id)
        {
            return _courseRepository.ExistsAsync(c => c.Id == id).Result;
        }
    }
}