using HealthGuard.Core.Entities.Identity;
using HealthGuard.Core.Services.contract;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace HealthGuard.Service.AuthService
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _configuration;

        public AuthService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<string> CreateTokenAsync(AppUser user, UserManager<AppUser> userManager)
        {

            var AuthClaims = new List<Claim>()
            {
                 new Claim(ClaimTypes.Name,user.DisplayName),
                 new Claim(ClaimTypes.Email,user.Email),
                 new Claim("BasketId", $"user_{user.Id}_cart"),
                 new Claim("PhoneNumber", user.PhoneNumber),
                 new Claim("IsAdmin", user.IsAdmin)
            };
            var userRole = await userManager.GetRolesAsync(user);
            foreach (var role in userRole)
            {
                AuthClaims.Add(new Claim(ClaimTypes.Role, role));
            }
            var AuthKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:AuthKey"] ?? string.Empty));
            var Token = new JwtSecurityToken(
                audience: _configuration["JWT:ValidAudiance"],
                issuer: _configuration["JWT:ValidIssuer"],
                expires: DateTime.Now.AddDays(double.Parse(_configuration["JWT:DurationInDays"] ?? "0")),
                claims: AuthClaims,
                signingCredentials: new SigningCredentials(AuthKey, SecurityAlgorithms.HmacSha256Signature)
                );
            return new JwtSecurityTokenHandler().WriteToken(Token);

        }
    }
}
