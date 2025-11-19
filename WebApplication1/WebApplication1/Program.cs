using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Configurations;
using WebApplication1.Middlewares;
using WebApplication1Data.Data;
using WebApplication1Data.Interfaces;
using WebApplication1Data.Models;
using WebApplication1Data.Repositories;
using WebApplication1.Services;
using Microsoft.AspNetCore.Authorization;
using WebApplication1.Authorization;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.Sources.Clear();
builder.Configuration.AddJsonFile("sharedsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddJsonFile("appsettings.Development.json", optional: true);
    builder.Configuration.AddUserSecrets<Program>(optional: true);
}
if (builder.Environment.IsProduction())
{
    builder.Configuration.AddJsonFile("appsettings.Production.json", optional: true);
    builder.Configuration.AddEnvironmentVariables();
}
// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<dbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// ������������� ��� User ���� ������ IdentityUser
builder.Services.AddDefaultIdentity<User>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 3;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
})
.AddRoles<IdentityRole>()  // ������ �������� �����
.AddEntityFrameworkStores<dbContext>();

//  авторизація з політиками
builder.Services.AddAuthorization(options =>
{
    //  Політика для перевірених клієнтів
    options.AddPolicy("IsVerifiedClient", policy =>
        policy.RequireClaim("IsVerifiedClient", "true"));
});

// Реєструємо обробник ресурсної авторизації
builder.Services.AddScoped<IAuthorizationHandler, MaterialAuthorizationHandler>();

// політику для редагування матеріалів
builder.Services.AddAuthorization(options =>
{
    //  Політика для редагування матеріалів (тільки викладач-автор)
    options.AddPolicy("CanEditMaterial", policy =>
        policy.Requirements.Add(new MaterialEditRequirement()));
});

// Реєструємо обробник для преміум-доступу
builder.Services.AddScoped<IAuthorizationHandler, MinimumWorkingHoursHandler>();

// Додаємо політику для преміум-доступу
builder.Services.AddAuthorization(options =>
{
   
    //  Політика для преміум-доступу (мінімум 100 робочих годин)
    options.AddPolicy("MinimumWorkingHours", policy =>
        policy.Requirements.Add(new MinimumWorkingHoursRequirement(100)));
});

// Реєструємо обробник для доступу до форуму
builder.Services.AddScoped<IAuthorizationHandler, ForumAccessHandler>();

// Додаємо політику для доступу до форуму
builder.Services.AddAuthorization(options =>
{
    // Політика для доступу до форуму (хоча б одне з тверджень)
    options.AddPolicy("ForumAccess", policy =>
        policy.Requirements.Add(new ForumAccessRequirement()));
});

builder.Services.AddControllersWithViews();
// ���������
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IGradeRepository, GradeRepository>();
builder.Services.AddScoped<ICourseRepository, CourseRepository>();
builder.Services.AddScoped<IStudentRepository, StudentRepository>();
builder.Services.AddScoped<ITeacherRepository, TeacherRepository>();
builder.Services.AddScoped<IScheduleRepository, ScheduleRepository>();
builder.Services.AddScoped<IMaterialRepository, MaterialRepository>();
builder.Services.AddScoped<ClaimService>();


builder.Services.Configure<AppConfiguration>(builder.Configuration.GetSection("AppSettings"));

builder.Services.AddSingleton(resolver =>
    resolver.GetRequiredService<Microsoft.Extensions.Options.IOptions<AppConfiguration>>().Value);



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.UseRequestLimiter(authLimit: 100, anonLimit: 20, window: TimeSpan.FromMinutes(1));



app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

// Seed Data
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await SeedData.Initialize(services);
}

app.Run();