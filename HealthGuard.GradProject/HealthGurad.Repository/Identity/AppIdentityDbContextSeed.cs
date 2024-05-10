using HealthGuard.Core.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthGurad.Repository.Identity
{
    public static class AppIdentityDbContextSeed
    {
        public static async Task SeedUsersAsync(UserManager<AppUser> userManager)
        {
            if (userManager.Users.Count() == 0)
            {
                var user = new AppUser()
                {
                    DisplayName = " AlaaMahmoud",
                    Email = "alaa.mahmoud@gmail.com",
                    UserName = "alaa.zaki",
                    Address = new Address() 
                    {
                        FName ="Alaa",
                        LName = "Mahmoud",
                        Street = "Helmya",
                        City="Cairo",
                        Country="Egypt"
                    },
                    PhoneNumber = "01054938265"
                };
                await userManager.CreateAsync(user, "Pa$$w0rd");
            }
        }
    }
}
