using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication1Data.Data;
using WebApplication1Data.models;

namespace WebApplication1.Controllers
{
    [Authorize]
    public class MaterialsController : Controller
    {
        private readonly dbContext _context;
        private readonly IAuthorizationService _authorizationService;

        public MaterialsController(dbContext context, IAuthorizationService authorizationService)
        {
            _context = context;
            _authorizationService = authorizationService;
        }

        // Список матеріалів - доступний всім авторизованим
        public async Task<IActionResult> Index()
        {
            var materials = await _context.Materials
                .Include(m => m.UploadedBy)
                .Include(m => m.Course)
                .ThenInclude(c => c.Teacher)
                .ThenInclude(t => t.User)
                .ToListAsync();

            return View(materials);
        }

        // Сторінка редагування - тільки для автора-викладача
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> Edit(int id)
        {
            var material = await _context.Materials
                .Include(m => m.UploadedBy)
                .Include(m => m.Course)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (material == null)
            {
                return NotFound();
            }

            // Перевірка ресурсної авторизації
            var authorizationResult = await _authorizationService.AuthorizeAsync(
                User, material, "CanEditMaterial");

            if (!authorizationResult.Succeeded)
            {
                return Forbid();
            }

            // Додаємо список курсів у ViewBag
            ViewBag.CourseList = await GetAvailableCoursesAsync();
            return View(material);
        }

        // POST метод для редагування
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> Edit(int id, Material model)
        {
            ModelState.Remove("UploadedBy");

            var material = await _context.Materials.FindAsync(id);
            if (material == null)
            {
                return NotFound();
            }

            // Перевірка ресурсної авторизації
            var authorizationResult = await _authorizationService.AuthorizeAsync(
                User, material, "CanEditMaterial");

            if (!authorizationResult.Succeeded)
            {
                return Forbid();
            }

            if (ModelState.IsValid)
            {
                material.Title = model.Title;
                material.Description = model.Description;
                material.ContentUrl = model.ContentUrl;
                material.CourseId = model.CourseId; // Змінили на CourseId

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Якщо модель не валідна, повертаємо View з даними
            ViewBag.CourseList = await GetAvailableCoursesAsync();
            return View(model);
        }

        // Створення нового матеріалу - тільки для викладачів
        [HttpGet]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> Create()
        {
            // Додаємо список курсів у ViewBag
            ViewBag.CourseList = await GetAvailableCoursesAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> Create(Material model)
        {
            ModelState.Remove("UploadedBy");
            if (ModelState.IsValid)
            {
                try
                {
                  

                    // Встановлюємо поточного користувача як автора
                    model.UploadedById = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                    _context.Materials.Add(model);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Матеріал успішно створено!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Помилка при створенні матеріалу: " + ex.Message);
                }
            }

            // Якщо модель не валідна, повертаємо View з даними
            ViewBag.CourseList = await GetAvailableCoursesAsync();
            return View(model);
        }

        // Деталі матеріалу - доступні всім
        public async Task<IActionResult> Details(int id)
        {
            var material = await _context.Materials
                .Include(m => m.UploadedBy)
                .Include(m => m.Course)
                .ThenInclude(c => c.Teacher)
                .ThenInclude(t => t.User)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (material == null)
            {
                return NotFound();
            }

            return View(material);
        }

        // Видалення - тільки для автора-викладача
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> Delete(int id)
        {
            var material = await _context.Materials
                .Include(m => m.UploadedBy)
                .Include(m => m.Course)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (material == null)
            {
                return NotFound();
            }

            var authorizationResult = await _authorizationService.AuthorizeAsync(
                User, material, "CanEditMaterial");

            if (!authorizationResult.Succeeded)
            {
                return Forbid();
            }

            return View(material);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var material = await _context.Materials.FindAsync(id);
            if (material == null)
            {
                return NotFound();
            }

            var authorizationResult = await _authorizationService.AuthorizeAsync(
                User, material, "CanEditMaterial");

            if (!authorizationResult.Succeeded)
            {
                return Forbid();
            }

            _context.Materials.Remove(material);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // Метод для отримання списку курсів
        private async Task<List<SelectListItem>> GetAvailableCoursesAsync()
        {
            var courses = await _context.Courses
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = $"{c.Title} ({c.GroupName})" // або просто c.Title
                })
                .ToListAsync();

            return courses;
        }
    }
}