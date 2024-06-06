using HealthGuard.Core.Entities;
using HealthGuard.Core.Entities.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthGuard.Core.Services.contract
{
    public interface IPaymentService 
    {
        Task<CustomerBasket?> CreateOrUpdatePaymentIntent(string basketId);
        Task<Order> UpdatePaymentIntentTosuccedOrFailed(string PaymentIntentId, bool isSuccess);
    }
}
