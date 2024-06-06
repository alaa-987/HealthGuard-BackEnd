using HealthGuard.Core.Entities.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthGuard.Core.Specifications.OrderSpecification
{
    public class OrdersWithItemsAndDeliverySpecification: BaseSpecifications<Order>
    {
        public OrdersWithItemsAndDeliverySpecification()
        {
            AddInclude(o => o.Items);
            AddInclude(o => o.DeliveryMethod);
        }
    }
}
