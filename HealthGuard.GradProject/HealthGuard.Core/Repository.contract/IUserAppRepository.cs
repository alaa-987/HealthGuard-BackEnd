using HealthGuard.Core.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthGuard.Core.Repository.contract
{
    public interface IUserAppRepository 
    {
        Task<IReadOnlyCollection<AppUser>> GetAllUsersAsync();
        Task<IReadOnlyCollection<AppUser>> GetAllNormalUsersAsync();
        Task DeleteUserAsync(string userId);
        Task<AppUser?> GetUserByIdAsync(string userId);
    }
}
