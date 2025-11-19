using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Controllers
{
    [Authorize(Policy = "IsVerifiedClient")] //  тільки для перевірених клієнтів
    public class ArchiveController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Documents()
        {
            return View();
        }
    }
}