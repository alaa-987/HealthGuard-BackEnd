using HealthGuard.Core.Entities.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthGuard.Core.Specifications.OrderSpecification
{
    public class OrderWithPaymentIntentSpec: BaseSpecifications<Order>
    {
        public OrderWithPaymentIntentSpec(string PaymentIntentId) : base(o => o.PaymentIntentId == PaymentIntentId)
        {

        }
    }
}
