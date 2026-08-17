using HeritageMarket.Domain.Entities;
using HeritageMarket.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HeritageMarket.Infrastructure.Persistence.Configurations;

public class BookAccessRequestConfiguration : IEntityTypeConfiguration<BookAccessRequest>
{
    public void Configure(EntityTypeBuilder<BookAccessRequest> builder)
    {
        builder.Property(r => r.ApplicationUserId).IsRequired();
        builder.Property(r => r.Reason).IsRequired().HasMaxLength(1000);
        builder.Property(r => r.PreferredCountry).IsRequired().HasMaxLength(100);
        builder.Property(r => r.AdminNote).HasMaxLength(500);

        builder.HasIndex(r => r.ApplicationUserId);

        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(r => r.ApplicationUserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
