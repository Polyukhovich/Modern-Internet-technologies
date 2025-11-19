using Microsoft.AspNetCore.Identity;
using WebApplication1Data.Models;

namespace WebApplication1.Services
{
    public class ClaimService
    {
        private readonly UserManager<User> _userManager;

        public ClaimService(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        // Метод для додавання claim "IsVerifiedClient"
        public async Task AddVerifiedClientClaimAsync(User user)
        {
            await _userManager.AddClaimAsync(user,
                new System.Security.Claims.Claim("IsVerifiedClient", "true"));
        }

        // Метод для додавання випадкових claims для демонстрації
        public async Task AddDemoClaimsAsync(User user)
        {
            var claims = new[]
            {
                new System.Security.Claims.Claim("IsVerifiedClient", "true"),
                new System.Security.Claims.Claim("WorkingHours", "150"),
                new System.Security.Claims.Claim("IsMentor", "true")
            };

            foreach (var claim in claims)
            {
                await _userManager.AddClaimAsync(user, claim);
            }
        }
    }
}