using Microsoft.AspNetCore.Mvc;
using WebApplication1.Configurations;


namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ConfigController : ControllerBase
    {
        private readonly AppConfiguration _appSettings;

        public ConfigController(AppConfiguration appSettings)
        {
            _appSettings = appSettings;
        }

        // Дія для виведення інформації про ApiKey
        [HttpGet("api-info")]
        public IActionResult ApiInfo()
        {
            var apiKey = _appSettings.ApiSettings.ApiKey;

            // Маскуємо ключ, щоб не показувати повністю
            var maskedKey = apiKey.Length > 10
                ? apiKey.Substring(0, 10) + "..." + apiKey.Substring(apiKey.Length - 4)
                : "****";

            // Отримуємо поточне середовище (Development або Production)
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Unknown";

            return Ok(new
            {
                ApiKeyPrefix = maskedKey,
                Environment = environment,
                Message = "ApiKey was successfully retrieved"
            });
        }
    }
}
