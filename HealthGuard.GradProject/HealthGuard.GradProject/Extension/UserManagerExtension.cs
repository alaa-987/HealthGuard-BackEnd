using HealthGuard.Core.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace HealthGuard.GradProject.Extension
{
    public static class UserManagerExtension
    {
        public static async Task<AppUser> FindUserByEmailWithAddressAsync(this UserManager<AppUser> userMnager, ClaimsPrincipal User)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var user = await userMnager.Users.Include(u => u.Address).FirstOrDefaultAsync(u => u.NormalizedEmail == email.ToUpper());
            return user;
        }
    }
}
