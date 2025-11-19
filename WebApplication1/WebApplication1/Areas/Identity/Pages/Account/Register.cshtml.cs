using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebApplication1.Services;
using WebApplication1Data.Data;
using WebApplication1Data.Models;
using WebApplication1Data.models;
using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly ClaimService _claimService;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly dbContext _context;

        public RegisterModel(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            ClaimService claimService,
            RoleManager<IdentityRole> roleManager,
            dbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _claimService = claimService;
            _roleManager = roleManager;
            _context = context;
        }
        [BindProperty]
        public InputModel Input { get; set; }

        public List<SelectListItem> AvailableGroups { get; set; } = new List<SelectListItem>();
        public string ReturnUrl { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "Email є обов'язковим")]
            [EmailAddress(ErrorMessage = "Невірний формат email")]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required(ErrorMessage = "Пароль є обов'язковим")]
            [StringLength(100, ErrorMessage = "Пароль повинен містити принаймні {2} символи.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Пароль")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Підтвердження пароля")]
            [Compare("Password", ErrorMessage = "Паролі не співпадають.")]
            public string ConfirmPassword { get; set; }

            [Required(ErrorMessage = "Ім'я є обов'язковим")]
            [Display(Name = "Ім'я")]
            public string FirstName { get; set; }

            [Required(ErrorMessage = "Прізвище є обов'язковим")]
            [Display(Name = "Прізвище")]
            public string LastName { get; set; }

            [Required(ErrorMessage = "Секретне слово є обов'язковим")]
            [Display(Name = "Секретне слово")]
            public string SecretWord { get; set; }

            // Поля для студента
            [Display(Name = "Група")]
            public string? GroupName { get; set; }

            // Додайте властивість Department для викладача
            [Display(Name = "Кафедра")]
            public string? Department { get; set; } // Додайте цей рядок
        
        }

        public void OnGet(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            InitializeGroups();
        }

        private void InitializeGroups()
        {
            // Список доступних груп з автоматичним визначенням курсу
            var groups = new List<string>
            {
                "КН-101", "КН-102", "КН-103",  // 1 курс
                "КН-201", "КН-202", "КН-203",  // 2 курс 
                "КН-301", "КН-302", "КН-303",  // 3 курс
                "КН-401", "КН-402", "КН-403",  // 4 курс
                "ПМ-101", "ПМ-102",            // 1 курс
                "ПМ-201", "ПМ-202",            // 2 курс
                "СА-101", "СА-102",            // 1 курс
                "СА-201", "СА-202"             // 2 курс
            };

            AvailableGroups = groups.Select(g => new SelectListItem
            {
                Value = g,
                Text = $"{g} ({GetCourseFromGroupName(g)} курс)" // Додаємо інформацію про курс
            }).ToList();

            AvailableGroups.Insert(0, new SelectListItem("-- Оберіть групу --", ""));
        }

        // Метод для автоматичного визначення курсу з назви групи
        private int GetCourseFromGroupName(string groupName)
        {
            if (string.IsNullOrEmpty(groupName) || groupName.Length < 4)
                return 1; // За замовчуванням 1 курс

            // Шукаємо цифру на третьому або четвертому місці (наприклад: "КН-101" -> "1")
            for (int i = 2; i < Math.Min(4, groupName.Length); i++)
            {
                if (char.IsDigit(groupName[i]))
                {
                    // Перша цифра в назві групи - це курс
                    int course = int.Parse(groupName[i].ToString());
                    return Math.Clamp(course, 1, 6); // Обмежуємо від 1 до 6 курсів
                }
            }

            return 1; // За замовчуванням 1 курс
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            InitializeGroups();

            if (ModelState.IsValid)
            {
                var user = new User
                {
                    UserName = Input.Email,
                    Email = Input.Email,
                    FirstName = Input.FirstName,
                    LastName = Input.LastName,
                    SecretWord = Input.SecretWord
                };

                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    string role = DetermineRole(Input.SecretWord);
                    await _userManager.AddToRoleAsync(user, role);

                    // Додаємо claim IsVerifiedClient при реєстрації
                    await _userManager.AddClaimAsync(user,
                        new System.Security.Claims.Claim("IsVerifiedClient", "true"));

                    if (role == "Student")
                    {
                        // Автоматично визначаємо курс з назви групи
                        int yearOfStudy = GetCourseFromGroupName(Input.GroupName);

                        var student = new Student
                        {
                            UserId = user.Id,
                            GroupName = Input.GroupName ?? "Не вказано",
                            YearOfStudy = yearOfStudy // Автоматично визначений курс
                        };
                        _context.Students.Add(student);
                    }
                    else if (role == "Teacher")
                    {
                        var teacher = new WebApplication1Data.models.Teacher
                        {
                            UserId = user.Id,
                            Department = Input.Department ?? "Не вказано"
                        };
                        _context.Teachers.Add(teacher);
                    }

                    await _context.SaveChangesAsync();
                    await AddRoleSpecificClaims(user, role, Input.GroupName);
                    await _claimService.AddVerifiedClientClaimAsync(user);
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return LocalRedirect(returnUrl);
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return Page();
        }

        private string DetermineRole(string secretWord)
        {
            switch (secretWord.ToLower().Trim())
            {
                case "student123":
                    return "Student";
                case "teacher456":
                    return "Teacher";
                case "admin789":
                    return "Admin";
                default:
                    return "User";
            }
        }

        private async Task AddRoleSpecificClaims(User user, string role, string groupName)
        {
            var claims = new List<System.Security.Claims.Claim>();

            switch (role)
            {
                case "Student":
                    claims.Add(new System.Security.Claims.Claim("IsVerifiedUser", "true"));
                    claims.Add(new System.Security.Claims.Claim("WorkingHours", "50"));
                    if (!string.IsNullOrEmpty(groupName))
                    {
                        claims.Add(new System.Security.Claims.Claim("Group", groupName));
                        // Додаємо також курс до claims
                        int course = GetCourseFromGroupName(groupName);
                        claims.Add(new System.Security.Claims.Claim("Course", course.ToString()));
                    }
                    break;
                case "Teacher":
                    claims.Add(new System.Security.Claims.Claim("IsMentor", "true"));
                    claims.Add(new System.Security.Claims.Claim("WorkingHours", "150"));
                    claims.Add(new System.Security.Claims.Claim("HasForumAccess", "true"));
                    break;
                case "Admin":
                    claims.Add(new System.Security.Claims.Claim("IsMentor", "true"));
                    claims.Add(new System.Security.Claims.Claim("WorkingHours", "200"));
                    claims.Add(new System.Security.Claims.Claim("HasForumAccess", "true"));
                    claims.Add(new System.Security.Claims.Claim("IsAdmin", "true"));
                    break;
            }

            foreach (var claim in claims)
            {
                await _userManager.AddClaimAsync(user, claim);
            }
        }
    }
}