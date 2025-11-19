using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Controllers
{
    [Authorize(Policy = "MinimumWorkingHours")] // тільки з достатньою кількістю годин
    public class PremiumController : Controller
    {
        public IActionResult Index()
        {
            // Отримуємо кількість годин для відображення
            var workingHoursClaim = User.FindFirst("WorkingHours");
            var workingHours = workingHoursClaim != null ? int.Parse(workingHoursClaim.Value) : 0;

            ViewBag.WorkingHours = workingHours;
            return View();
        }

        public IActionResult ExclusiveContent()
        {
            return View();
        }

        public IActionResult AdvancedFeatures()
        {
            return View();
        }
    }
}