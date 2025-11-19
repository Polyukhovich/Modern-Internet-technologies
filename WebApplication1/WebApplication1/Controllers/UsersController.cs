using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using WebApplication1Data.Interfaces;

namespace WebApplication1.Controllers;
public class UsersController : Controller
{
    private readonly IUserRepository _userRepository;

    //  Інжектимо репозиторій через конструктор (Dependency Injection)
    public UsersController(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    //  Отримати всіх користувачів (метод Index)
    public async Task<IActionResult> Index()
    {
        var users = await _userRepository.GetAllAsync();
        return View(users);
    }

    // Пошук користувача за email
    public async Task<IActionResult> Details(string email)
    {
        if (string.IsNullOrEmpty(email))
            return BadRequest("Email не вказано");

        var user = await _userRepository.GetByEmailAsync(email);
        if (user == null)
            return NotFound("Користувача не знайдено");

        return View(user);
    }

    //Приклад перевірки існування користувача
    public async Task<IActionResult> CheckEmail(string email)
    {
        bool exists = await _userRepository.ExistsAsync(u => u.Email == email);
        return Content(exists ? "Користувач з таким email існує" : "Email вільний");
    }
}