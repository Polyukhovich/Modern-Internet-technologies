using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using WebApplication1.Configurations;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppConfiguration _appSettings; // Додаємо поле для збереження конфігурації
        private readonly IStringLocalizer<HomeController> _localizer;  //  ДОДАЛИ ЛОКАЛІЗАТОР

        //Додаємо AppSettings у конструктор через Dependency Injection
        public HomeController(ILogger<HomeController> logger, AppConfiguration appSettings, IStringLocalizer<HomeController> localizer) //  ДОДАЛИ)
        
        {
            _logger = logger;
            _appSettings = appSettings;
            _localizer = localizer;  //  ДОДАЛИ
        }
        [AllowAnonymous] 
        //Використовуємо AppSettings у дії Index
        public IActionResult Index()
        {

            // Використання ресурсів через _localizer
            ViewBag.LocalizedText = _localizer["Test"];               //  ДОДАЛИ (ТЕКСТ З РЕСУРСУ)
            ViewBag.ChangeLang = _localizer["lblChangeLanguage"];    //  ДОДАЛИ

            // Передаємо дані у View через ViewBag
            ViewBag.AppName = _appSettings.ApplicationName;
            ViewBag.Theme = _appSettings.Theme;
            ViewBag.ApiKey = _appSettings.ApiSettings?.ApiKey;
            ViewBag.DefaultRole = _appSettings.DefaultRole;



            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
