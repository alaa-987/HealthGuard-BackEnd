using HealthGuard.Core.Entities.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthGuard.Core.Services.contract
{
    public interface IOrderService
    {
        Task<Order?> CreateOrderAsync(string BuyerEmail, string BasketId, int DeliveryMethodId, ShippingAddress ShippingAddress);
        Task<IReadOnlyList<Order>> CreateOrderForSpecUserAsync(string BuyerEmail);
        Task<Order> CreateOrderByIdForSpecUserAsync(string BuyerEmail, int OrderId);
    }
}
