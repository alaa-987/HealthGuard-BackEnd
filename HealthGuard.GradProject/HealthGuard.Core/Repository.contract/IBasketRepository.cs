﻿using HealthGuard.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthGuard.Core.Repository.contract
{
    public interface IBasketRepository
    {
        Task<CustomerBasket> GetBasketAsync(string basketId);
        Task<CustomerBasket> UpdateBasketAsync(CustomerBasket basket);
        Task<CustomerBasket> CreateOrUpdateBasketAsync(CustomerBasket basket);

        Task<bool> DeleteBasketAsync(string basketId);
        Task<bool> UpdateBasketItemQuantityAsync(string basketId, int itemId, int newQuantity);
        Task<bool> RemoveBasketItemAsync(string basketId, int itemId);
    }
}
