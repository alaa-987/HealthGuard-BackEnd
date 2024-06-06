using HealthGuard.Core.Entities;
using HealthGuard.Core.Repository.contract;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace HealthGurad.Repository
{
    public class WishListRepository : IWishListRepository
    {
        private readonly StackExchange.Redis.IDatabase _dataBase;

        public WishListRepository(IConnectionMultiplexer redis)
        {
            _dataBase = redis.GetDatabase();
        }

        public async Task<CustomerWishList> CreateOrUpdateWishListAsync(CustomerWishList wishList)
        {
            var serializedBasket = JsonSerializer.Serialize(wishList);

            var createdOrUpdated = await _dataBase.StringSetAsync(wishList.Id, serializedBasket, TimeSpan.FromDays(30));

            if (createdOrUpdated)
            {
                return await GetWishlistAsync(wishList.Id);
            }
            else
            {
                return null;
            }
        }

        public async Task<bool> DeleteWishListAsync(string wishListId)
        {
            return await _dataBase.KeyDeleteAsync(wishListId);
        }

        public async Task<CustomerWishList> GetWishlistAsync(string wishListId)
        {
            var WishList = await _dataBase.StringGetAsync(wishListId);
            return WishList.IsNullOrEmpty ? null : JsonSerializer.Deserialize<CustomerWishList>(WishList);
        }

        public async Task<CustomerWishList> UpdateWishlistAsync(CustomerWishList wishList)
        {
            var creativeUpdated = await _dataBase.StringSetAsync(wishList.Id, JsonSerializer.Serialize(wishList), TimeSpan.FromDays(30));
            if (creativeUpdated is false)
            {
                return null;
            }
            else
            {
                return await GetWishlistAsync(wishList.Id);
            }
        }

        public async Task<bool> UpdateWishListItemQuantityAsync(string wishListId, int itemId, int newQuantity)
        {
            var wishList = await GetWishlistAsync(wishListId);
            if (wishList == null)
                return false;

            var itemToUpdate = wishList.Items.FirstOrDefault(item => item.Id == itemId);
            if (itemToUpdate == null)
                return false;

            itemToUpdate.Quanntity = newQuantity;

            await UpdateWishlistAsync(wishList);
            return true;
        }
    }
}
