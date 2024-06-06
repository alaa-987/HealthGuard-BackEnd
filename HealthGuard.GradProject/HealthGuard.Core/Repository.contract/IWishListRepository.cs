using HealthGuard.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthGuard.Core.Repository.contract
{
    public interface IWishListRepository
    {
        Task<CustomerWishList> GetWishlistAsync(string wishListId);
        Task<CustomerWishList> UpdateWishlistAsync(CustomerWishList wishList);
        Task<CustomerWishList> CreateOrUpdateWishListAsync(CustomerWishList wishList);
        Task<bool> DeleteWishListAsync(string wishListId);
        Task<bool> UpdateWishListItemQuantityAsync(string wishListId, int itemId, int newQuantity);
    }
}
