using HealthGuard.Core.Entities;
using HealthGuard.Core.Repository.contract;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace HealthGurad.Repository
{
    public class BasketRepository: IBasketRepository
    {

        private readonly StackExchange.Redis.IDatabase _dataBase;

        public BasketRepository(IConnectionMultiplexer redis)
        {
            _dataBase = redis.GetDatabase();
        }

        public async Task<CustomerBasket> CreateOrUpdateBasketAsync(CustomerBasket basket)
        {
            var serializedBasket = JsonSerializer.Serialize(basket);

            var createdOrUpdated = await _dataBase.StringSetAsync(basket.Id, serializedBasket, TimeSpan.FromDays(30));

            if (createdOrUpdated)
            {
                return await GetBasketAsync(basket.Id);
            }
            else
            {
                return null;
            }
        }

        public async Task<bool> DeleteBasketAsync(string basketId)
        {
            return await _dataBase.KeyDeleteAsync(basketId);
        }

        public async Task<CustomerBasket> GetBasketAsync(string basketId)
        {
            var basketData = await _dataBase.StringGetAsync(basketId);
            if (basketData.IsNullOrEmpty)
            {
                return null;
            }
            return JsonSerializer.Deserialize<CustomerBasket>(basketData);
        }
        public async Task<bool> RemoveBasketItemAsync(string basketId, int itemId)
        {
            var basket = await GetBasketAsync(basketId);

            if (basket == null)
            {
                return false;
            }

            var itemToRemove = basket.Items.FirstOrDefault(item => item.Id == itemId);

            if (itemToRemove == null)
            {
                return false;
            }

            basket.Items.Remove(itemToRemove);

            var updatedBasket = await UpdateBasketAsync(basket);

            if (updatedBasket == null)
            {
                return false;
            }

            return true;
        }

        public async Task<CustomerBasket> UpdateBasketAsync(CustomerBasket basket)
        {
            var creativeUpdated = await _dataBase.StringSetAsync(basket.Id, JsonSerializer.Serialize(basket), TimeSpan.FromDays(30));
            if (creativeUpdated is false)
            {
                return null;
            }
            else
            {
                return await GetBasketAsync(basket.Id);
            }
        }
        public async Task<bool> UpdateBasketItemQuantityAsync(string basketId, int itemId, int newQuantity)
        {
            var basket = await GetBasketAsync(basketId);
            if (basket == null)
                return false;

            var itemToUpdate = basket.Items.FirstOrDefault(item => item.Id == itemId);
            if (itemToUpdate == null)
                return false; 

            itemToUpdate.Quanntity = newQuantity;

            await UpdateBasketAsync(basket);
            return true;
        }

    }
}
