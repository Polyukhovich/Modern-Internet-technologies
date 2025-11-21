using Azure;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

public class SelectLanguageController : Controller
{
    private readonly IOptions<RequestLocalizationOptions> LocOptions;

    public SelectLanguageController(IOptions<RequestLocalizationOptions> locOptions)
    {
        LocOptions = locOptions;
    }

    public IActionResult Index(string returnUrl)
    {
        ViewData["ReturnUrl"] = returnUrl;
        var cultures = LocOptions.Value.SupportedUICultures!.ToList();
        return View(cultures);
    }

    [HttpPost]
    public IActionResult SetLanguage(string cultureName, string returnUrl)
    {
        Response.Cookies.Append(
            CookieRequestCultureProvider.DefaultCookieName,
            CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(cultureName)),
            new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
        );

        return string.IsNullOrEmpty(returnUrl)
            ? RedirectToAction("Index", "Home")
            : LocalRedirect(returnUrl);
    }
}
