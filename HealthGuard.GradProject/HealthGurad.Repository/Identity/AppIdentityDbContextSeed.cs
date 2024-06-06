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
        public static async Task SeedUsersAsync(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            if (!await roleManager.RoleExistsAsync("Nurse"))
            {
                await roleManager.CreateAsync(new IdentityRole("Nurse"));
            }
            if (!await roleManager.RoleExistsAsync("User"))
            {
                await roleManager.CreateAsync(new IdentityRole("User"));
            }
            if (!userManager.Users.Any(u => u.UserName == "normal.user"))
            {
                var normalUser = new AppUser()
                {
                    DisplayName = "Normal User",
                    Email = "normal.user@gmail.com",
                    UserName = "normal.user",
                    Address = new Address()
                    {
                        FName = "Normal",
                        LName = "User",
                        Street = "456 Elm St",
                        City = "Somewhere",
                        Country = "USA"
                    },
                    PhoneNumber = "0987654321"
                };

                await userManager.CreateAsync(normalUser, "Pa$$w0rd");
                await userManager.AddToRoleAsync(normalUser, "User");
            }
            if (!userManager.Users.Any(u => u.UserName == "nurse.jane"))
            {
                var nurseUser = new AppNurse()
                {
                    DisplayName = "Nurse Jane",
                    Email = "nurse.jane@gmail.com",
                    UserName = "nurse.jane",
                    Address = new Address()
                    {
                        FName = "Jane",
                        LName = "Doe",
                        Street = "123 Main St",
                        City = "Somewhere",
                        Country = "USA"
                    },
                    PhoneNumber = "1234567890",
                    Price = 100.0m,
                    Description = "Experienced nurse specializing in pediatric care",
                    Hospital = "City Hospital",
                    Specialty = "Pediatrics"
                };

                await userManager.CreateAsync(nurseUser, "Pa$$w0rd");
                await userManager.AddToRoleAsync(nurseUser, "Nurse");
            }
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
