using Microsoft.AspNetCore.Identity;
using WebApplication1Data.Data;
using WebApplication1Data.Models;
using WebApplication1Data.models;
using Microsoft.Extensions.DependencyInjection;

namespace WebApplication1Data.Data
{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<User>>();
            var context = serviceProvider.GetRequiredService<dbContext>();

            // Створюємо ролі
            string[] roles = { "Admin", "Teacher", "Student" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }


            // Створюємо тестового викладача
            var teacherEmail = "teacher@test.com";
            var teacherUser = await userManager.FindByEmailAsync(teacherEmail);
            if (teacherUser == null)
            {
                teacherUser = new User
                {
                    UserName = teacherEmail,
                    Email = teacherEmail,
                    FirstName = "John",
                    LastName = "Teacher"
                };
                await userManager.CreateAsync(teacherUser, "Teacher123!");
                await userManager.AddToRoleAsync(teacherUser, "Teacher");

                // Створюємо запис у таблиці Teachers
                context.Teachers.Add(new Teacher
                {
                    UserId = teacherUser.Id,
                    Department = "Computer Science"
                });
                await context.SaveChangesAsync();
            }

            // Створюємо тестового студента
            var studentEmail = "student@test.com";
            var studentUser = await userManager.FindByEmailAsync(studentEmail);
            if (studentUser == null)
            {
                studentUser = new User
                {
                    UserName = studentEmail,
                    Email = studentEmail,
                    FirstName = "Alice",
                    LastName = "Student"
                };
                await userManager.CreateAsync(studentUser, "Student123!");
                await userManager.AddToRoleAsync(studentUser, "Student");

                // Створюємо запис у таблиці Students
                context.Students.Add(new Student
                {
                    UserId = studentUser.Id,
                    GroupName = "CS-101",
                    YearOfStudy = 1
                });
                await context.SaveChangesAsync();
            }
        }
    }
}