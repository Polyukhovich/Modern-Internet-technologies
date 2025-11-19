using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using WebApplication1Data.models;
using WebApplication1Data.Models;

namespace WebApplication1.Authorization
{
    public class MaterialAuthorizationHandler : AuthorizationHandler<IAuthorizationRequirement, Material>
    {
        private readonly UserManager<User> _userManager;

        public MaterialAuthorizationHandler(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            IAuthorizationRequirement requirement,
            Material resource)
        {
            // Отримуємо поточного користувача
            var currentUser = await _userManager.GetUserAsync(context.User);

            if (currentUser == null)
                return;

            // Перевіряємо, чи поточний користувач є викладачем
            var isTeacher = await _userManager.IsInRoleAsync(currentUser, "Teacher");

            // Перевіряємо, чи поточний користувач є автором матеріалу
            var isAuthor = resource.UploadedById == currentUser.Id;

            // Доступ мають тільки викладачі, які є авторами матеріалу
            if (isTeacher && isAuthor)
            {
                context.Succeed(requirement);
            }
        }
    }
}