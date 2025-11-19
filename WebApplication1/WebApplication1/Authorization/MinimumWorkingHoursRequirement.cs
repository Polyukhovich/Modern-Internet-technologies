using Microsoft.AspNetCore.Authorization;

namespace WebApplication1.Authorization
{
    public class MinimumWorkingHoursRequirement : IAuthorizationRequirement
    {
        public int MinimumHours { get; }

        public MinimumWorkingHoursRequirement(int minimumHours)
        {
            MinimumHours = minimumHours;
        }
    }
}