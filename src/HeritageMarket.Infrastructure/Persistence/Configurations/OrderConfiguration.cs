using HeritageMarket.Domain.Entities;
using HeritageMarket.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HeritageMarket.Infrastructure.Persistence.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.Property(o => o.ApplicationUserId).IsRequired();
        builder.Property(o => o.TotalAmount).HasPrecision(18, 2);
        builder.Property(o => o.ShippingAddress).IsRequired().HasMaxLength(300);
        builder.Property(o => o.ShippingCity).IsRequired().HasMaxLength(100);

        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(o => o.ApplicationUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.Property(i => i.UnitPrice).HasPrecision(18, 2);

        builder.HasOne(i => i.Order)
            .WithMany(o => o.Items)
            .HasForeignKey(i => i.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(i => i.Product)
            .WithMany(p => p.OrderItems)
            .HasForeignKey(i => i.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
