using HealthGuard.Core.Entities.Order;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthGurad.Repository.Data.Configuration
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.OwnsOne(o => o.ShippingAddress, shippingaddress => shippingaddress.WithOwner());
            builder.Property(o => o.Status)
                   .HasConversion(
                    os => os.ToString(),
                    os => (OrderStatus)Enum.Parse(typeof(OrderStatus), os));
            builder.Property(o => o.SubTotal)
                   .HasColumnType("decimal(18,2)");
            builder.HasOne(o => o.DeliveryMethod)
                   .WithMany()
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
