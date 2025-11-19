using Microsoft.AspNetCore.Authorization;

namespace WebApplication1.Authorization
{
    public class MinimumWorkingHoursHandler : AuthorizationHandler<MinimumWorkingHoursRequirement>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            MinimumWorkingHoursRequirement requirement)
        {
            // Шукаємо claim WorkingHours у користувача
            var workingHoursClaim = context.User.FindFirst("WorkingHours");

            if (workingHoursClaim != null &&
                int.TryParse(workingHoursClaim.Value, out int workingHours) &&
                workingHours >= requirement.MinimumHours)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}