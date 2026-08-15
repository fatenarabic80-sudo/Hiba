using HeritageMarket.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HeritageMarket.Infrastructure.Persistence.Configurations;

public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.Property(n => n.Message).IsRequired().HasMaxLength(500);

        builder.HasOne(n => n.Product)
            .WithMany()
            .HasForeignKey(n => n.ProductId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
