using Microsoft.AspNetCore.Authorization;

namespace WebApplication1.Authorization
{
    public class ForumAccessHandler : AuthorizationHandler<ForumAccessRequirement>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            ForumAccessRequirement requirement)
        {
            //  Перевіряємо хоча б одне з тверджень
            if (context.User.HasClaim(c => c.Type == "IsMentor" && c.Value == "true") ||
                context.User.HasClaim(c => c.Type == "IsVerifiedUser" && c.Value == "true") ||
                context.User.HasClaim(c => c.Type == "HasForumAccess" && c.Value == "true"))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}