using HealthGuard.Core.Entities.Identity;
using HealthGuard.Core.Repository.contract;
using HealthGurad.Repository.Data;
using HealthGurad.Repository.Data.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthGurad.Repository
{
    public class UserRepository : IUserAppRepository
    {
        private readonly AppIDentityDbContext _dbContext;

        public UserRepository(AppIDentityDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task DeleteUserAsync(string userId)
        {
            var user = await _dbContext.Users.FindAsync(userId);
            if (user != null)
            {
                _dbContext.Users.Remove(user);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task<IReadOnlyCollection<AppUser>> GetAllNormalUsersAsync()
        {
            return await _dbContext.Users.Where(u => u.IsAdmin == "false").ToListAsync();
        }

        public async Task<IReadOnlyCollection<AppUser>> GetAllUsersAsync()
        {
            var users = await _dbContext.Users
                      .Where(u => !(u is AppNurse) || !string.IsNullOrWhiteSpace(((AppNurse)u).NurseName))
                      .ToListAsync();

            return users;
        }

        public async Task<AppUser?> GetUserByIdAsync(string userId)
        {
            return await _dbContext.Users.FindAsync(userId);
        }
    }
}
