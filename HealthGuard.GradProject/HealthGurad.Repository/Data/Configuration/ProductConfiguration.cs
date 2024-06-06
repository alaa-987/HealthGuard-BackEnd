using HealthGuard.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace HealthGurad.Repository.Data.Configuration
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.Property(p => p.Name).IsRequired().HasMaxLength(100);
            builder.Property(p => p.PictureUrl).IsRequired();
            builder.Property(p => p.Price)
                   .HasColumnType("decimal(18,2)");
            builder.Property(p => p.Rate)
                   .HasColumnType("decimal(18,2)");
            builder.HasOne(p => p.Category)
                   .WithMany()
                   .HasForeignKey(p => p.CategoryId);
            builder.Property(p => p.Rate)
                   .HasColumnName("NewRate");
          //builder.HasOne(p => p.WishList)
          //       .WithMany(w => w.Products)
          //       .HasForeignKey(p => p.WishlistId)
          //       .IsRequired(false);
        }
    }
}
