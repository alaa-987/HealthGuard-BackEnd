using HealthGuard.Core.Entities.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthGuard.Core.Specifications.OrderSpecification
{
    public class OrdersWithItemsAndOrderingSpecification: BaseSpecifications<Order>
    {
        public OrdersWithItemsAndOrderingSpecification(int orderId)
       : base(o => o.Id == orderId)
        {
            AddInclude(o => o.Items);
            AddInclude(o => o.DeliveryMethod);
        }
    }
}
