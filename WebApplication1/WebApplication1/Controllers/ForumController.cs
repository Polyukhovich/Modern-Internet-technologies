using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Controllers
{
    [Authorize(Policy = "ForumAccess")] // тільки з відповідними claims
    public class ForumController : Controller
    {
        public IActionResult Index()
        {
            // Визначаємо тип доступу для відображення
            ViewBag.AccessType = GetAccessType();
            return View();
        }

        public IActionResult Discussions()
        {
            return View();
        }

        public IActionResult Topics()
        {
            return View();
        }

        public IActionResult CreateTopic()
        {
            return View();
        }

        private string GetAccessType()
        {
            if (User.HasClaim(c => c.Type == "IsMentor" && c.Value == "true"))
                return "Ментор";
            else if (User.HasClaim(c => c.Type == "IsVerifiedUser" && c.Value == "true"))
                return "Верифікований користувач";
            else if (User.HasClaim(c => c.Type == "HasForumAccess" && c.Value == "true"))
                return "Спеціальний доступ";
            else
                return "Користувач";
        }
    }
}