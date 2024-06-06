using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthGuard.Core.Entities.Order
{
    public class Order: BaseEntity
    {
        public Order()
        {

        }
        public Order(string buyeremail, ShippingAddress shippingaddress,DeliveryMethod deliveryMethod,  ICollection<OrderItem> items, decimal subtotal/*, string paymentIntentId*/)
        {
            BuyerEmail = buyeremail;
            ShippingAddress = shippingaddress;
            Items = items;
            SubTotal = subtotal;
           // PaymentIntentId = paymentIntentId;
            DeliveryMethod = deliveryMethod;

        }
        public string BuyerEmail { get; set; }
        public DateTimeOffset OrderDate { get; set; } = DateTimeOffset.UtcNow;
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        public ShippingAddress ShippingAddress { get; set; }
        public DeliveryMethod DeliveryMethod { get; set; }
        // public int DeliveryMethodId { get; set; }
        public ICollection<OrderItem> Items { get; set; } = new HashSet<OrderItem>();

        public decimal SubTotal { get; set; }
        [NotMapped]
        public decimal Total { get; }
        public decimal GetTotal() => SubTotal + DeliveryMethod.Cost;
        public string? PaymentIntentId { get; set; }
    }
}
