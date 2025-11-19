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
    public class GradesController : Controller
    {
        private readonly IGradeRepository _gradeRepository;
        private readonly ITeacherRepository _teacherRepository;
        private readonly ICourseRepository _courseRepository;
        private readonly IStudentRepository _studentRepository;
        private readonly UserManager<User> _userManager;

        public GradesController(
            IGradeRepository gradeRepository,
            ITeacherRepository teacherRepository,
            ICourseRepository courseRepository,
            IStudentRepository studentRepository,
            UserManager<User> userManager)
        {
            _gradeRepository = gradeRepository;
            _teacherRepository = teacherRepository;
            _courseRepository = courseRepository;
            _studentRepository = studentRepository;
            _userManager = userManager;
        }

        // -----------------------------------------------------------------------------
        // ✅ 1. Отримання поточного викладача - ВИПРАВЛЕНО
        // -----------------------------------------------------------------------------
        private async Task<TeacherEntity?> GetCurrentTeacherAsync()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) return null;

            return await _teacherRepository.GetByUserIdAsync(currentUser.Id);
        }

        // -----------------------------------------------------------------------------
        // ✅ 2. LoadGroupsAsync() — через репозиторій студентів
        // -----------------------------------------------------------------------------
        private async Task LoadGroupListAsync()
        {
            var students = await _studentRepository.GetStudentsWithUserInfoAsync();
            var groups = students
                .Where(s => !string.IsNullOrEmpty(s.GroupName))
                .Select(s => s.GroupName.Trim())
                .Distinct()
                .OrderBy(g => g)
                .ToList();

            ViewBag.GroupList = new SelectList(groups);
        }

        // -----------------------------------------------------------------------------
        // ✅ 3. Index — список оцінок - ВИПРАВЛЕНО
        // -----------------------------------------------------------------------------
        public async Task<IActionResult> Index(int? courseId)
        {
            var teacher = await GetCurrentTeacherAsync();
            if (teacher == null) return Forbid();

            var courses = await _courseRepository.GetTeacherCoursesAsync(teacher.Id);

            // Отримуємо оцінки через репозиторій
            var grades = await _gradeRepository.GetTeacherGradesAsync(teacher.Id);

            // Фільтруємо по курсу якщо потрібно
            if (courseId.HasValue)
            {
                grades = grades.Where(g => g.CourseId == courseId.Value).ToList();
            }

            ViewBag.Courses = new SelectList(courses, "Id", "Title", courseId);
            ViewBag.SelectedCourseId = courseId;

            return View(grades);
        }

        // -----------------------------------------------------------------------------
        // ✅ 4. Details - ВИПРАВЛЕНО
        // -----------------------------------------------------------------------------
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return RedirectToAction(nameof(Index));

            var teacher = await GetCurrentTeacherAsync();
            if (teacher == null) return Forbid();

            var grade = await _gradeRepository.GetGradeWithDetailsAsync(id.Value);

            if (grade == null || grade.TeacherId != teacher.Id)
            {
                TempData["ErrorMessage"] = "Оцінку не знайдено";
                return RedirectToAction(nameof(Index));
            }

            return View(grade);
        }

        // -----------------------------------------------------------------------------
        // ✅ 5. Create (GET) - ВИПРАВЛЕНО
        // -----------------------------------------------------------------------------
        public async Task<IActionResult> Create()
        {
            var teacher = await GetCurrentTeacherAsync();
            if (teacher == null) return Forbid();

            // Курси викладача через репозиторій
            var courses = await _courseRepository.GetTeacherCoursesAsync(teacher.Id);
            ViewBag.Courses = new SelectList(courses, "Id", "Title");

            // Групи студентів через репозиторій
            await LoadGroupListAsync();

            return View(new Grade());
        }

        // -----------------------------------------------------------------------------
        // ✅ 6. Create (POST) - ВИПРАВЛЕНО
        // -----------------------------------------------------------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Grade grade)
        {
            var teacher = await GetCurrentTeacherAsync();
            if (teacher == null) return Forbid();

            // Встановлюємо дату і викладача
            grade.DateSet = DateTime.Now;
            grade.TeacherId = teacher.Id;

            // Видаляємо GroupName з ModelState, бо його немає в БД
            ModelState.Remove("GroupName");
            ModelState.Remove("Course");
            ModelState.Remove("Student");
            ModelState.Remove("Teacher");

            if (ModelState.IsValid)
            {
                try
                {
                    await _gradeRepository.AddAsync(grade);
                    await _gradeRepository.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Оцінку успішно додано";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Помилка при збереженні: {ex.Message}");
                }
            }

            // Відновлення ViewBag при помилці
            var courses = await _courseRepository.GetTeacherCoursesAsync(teacher.Id);
            ViewBag.Courses = new SelectList(courses, "Id", "Title");
            await LoadGroupListAsync();

            return View(grade);
        }

        // -----------------------------------------------------------------------------
        // ✅ 7. AJAX — Отримати студентів по групі
        // -----------------------------------------------------------------------------
        [HttpGet]
        public async Task<JsonResult> GetStudentsForGroup(string groupName)
        {
            if (string.IsNullOrWhiteSpace(groupName))
                return Json(new { success = false, message = "Назва групи порожня" });

            var students = await _studentRepository.GetStudentsWithUserInfoAsync();
            var filteredStudents = students
                .Where(s => s.GroupName.Trim().ToLower() == groupName.Trim().ToLower())
                .Select(s => new
                {
                    id = s.Id,
                    name = s.User != null
                        ? (!string.IsNullOrEmpty(s.User.FullName) ? s.User.FullName : s.User.UserName)
                        : "(без імені)"
                })
                .OrderBy(s => s.name)
                .ToList();

            return Json(new { success = true, students = filteredStudents });
        }

        // -----------------------------------------------------------------------------
        // ✅ 8. Edit (GET) - ВИПРАВЛЕНО
        // -----------------------------------------------------------------------------
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return RedirectToAction(nameof(Index));

            var teacher = await GetCurrentTeacherAsync();
            if (teacher == null) return Forbid();

            var grade = await _gradeRepository.GetGradeWithDetailsAsync(id.Value);

            if (grade == null || grade.TeacherId != teacher.Id)
            {
                TempData["ErrorMessage"] = "Оцінку не знайдено";
                return RedirectToAction(nameof(Index));
            }

            // Завантажуємо курси для випадаючого списку
            var courses = await _courseRepository.GetTeacherCoursesAsync(teacher.Id);
            ViewBag.CourseList = new SelectList(courses, "Id", "Title", grade.CourseId);

            // Завантажуємо студентів для випадаючого списку
            var students = await _studentRepository.GetStudentsWithUserInfoAsync();
            var courseStudents = students
                .Where(s => s.GroupName == grade.Student?.GroupName)
                .OrderBy(s => s.User.LastName)
                .ThenBy(s => s.User.FirstName)
                .Select(s => new {
                    Id = s.Id,
                    FullName = $"{s.User.LastName} {s.User.FirstName}"
                })
                .ToList();

            ViewBag.StudentList = new SelectList(courseStudents, "Id", "FullName", grade.StudentId);

            return View(grade);
        }

        // -----------------------------------------------------------------------------
        // ✅ 9. Edit (POST) - ВИПРАВЛЕНО
        // -----------------------------------------------------------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, IFormCollection form)
        {
            var teacher = await GetCurrentTeacherAsync();
            if (teacher == null) return Forbid();

            var grade = await _gradeRepository.GetByIdAsync(id);

            if (grade == null || grade.TeacherId != teacher.Id)
            {
                TempData["ErrorMessage"] = "Оцінку не знайдено";
                return RedirectToAction(nameof(Index));
            }

            // Оновлюємо значення з форми
            if (int.TryParse(form["CourseId"], out int courseId))
                grade.CourseId = courseId;

            if (int.TryParse(form["StudentId"], out int studentId))
                grade.StudentId = studentId;

            if (decimal.TryParse(form["Value"], out decimal value))
                grade.Value = value;
            else
                ModelState.AddModelError("Value", "Некоректна оцінка");

            grade.DateSet = DateTime.Now;

            ModelState.Remove("Course");
            ModelState.Remove("Student");
            ModelState.Remove("Teacher");

            if (ModelState.IsValid)
            {
                try
                {
                    await _gradeRepository.UpdateAsync(grade);
                    await _gradeRepository.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Оцінку оновлено.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Помилка при збереженні: {ex.Message}");
                }
            }

            // Перезавантаження ViewBag при помилці
            var courses = await _courseRepository.GetTeacherCoursesAsync(teacher.Id);
            ViewBag.CourseList = new SelectList(courses, "Id", "Title", grade.CourseId);

            var students = await _studentRepository.GetStudentsWithUserInfoAsync();
            var currentStudent = students.FirstOrDefault(s => s.Id == grade.StudentId);
            var courseStudents = students
                .Where(s => s.GroupName == currentStudent?.GroupName)
                .OrderBy(s => s.User.LastName)
                .ThenBy(s => s.User.FirstName)
                .Select(s => new {
                    Id = s.Id,
                    FullName = $"{s.User.LastName} {s.User.FirstName}"
                })
                .ToList();

            ViewBag.StudentList = new SelectList(courseStudents, "Id", "FullName", grade.StudentId);

            return View(grade);
        }

        // -----------------------------------------------------------------------------
        // ✅ 10. Delete (GET) - ВИПРАВЛЕНО
        // -----------------------------------------------------------------------------
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return RedirectToAction(nameof(Index));

            var teacher = await GetCurrentTeacherAsync();
            if (teacher == null) return Forbid();

            var grade = await _gradeRepository.GetGradeWithDetailsAsync(id.Value);

            if (grade == null || grade.TeacherId != teacher.Id)
            {
                TempData["ErrorMessage"] = "Оцінку не знайдено";
                return RedirectToAction(nameof(Index));
            }

            return View(grade);
        }

        // -----------------------------------------------------------------------------
        // ✅ 11. DeleteConfirmed - ВИПРАВЛЕНО
        // -----------------------------------------------------------------------------
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var teacher = await GetCurrentTeacherAsync();
            if (teacher == null) return Forbid();

            var grade = await _gradeRepository.GetByIdAsync(id);

            if (grade == null || grade.TeacherId != teacher.Id)
            {
                TempData["ErrorMessage"] = "Оцінку не знайдено";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                _gradeRepository.Delete(grade);
                await _gradeRepository.SaveChangesAsync();

                TempData["SuccessMessage"] = "Оцінку видалено.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Помилка при видаленні: {ex.Message}";
                return RedirectToAction(nameof(Delete), new { id });
            }
        }
    }
}