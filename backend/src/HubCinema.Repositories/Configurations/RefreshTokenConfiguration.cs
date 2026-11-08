using HubCinema.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HubCinema.Repositories.Configurations;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.Property(t => t.Token).HasMaxLength(512).IsRequired();
        builder.HasIndex(t => t.Token);

        builder.HasOne(t => t.User)
            .WithMany(u => u.RefreshTokens)
            .HasForeignKey(t => t.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Computed properties are not columns.
        builder.Ignore(t => t.IsRevoked);
        builder.Ignore(t => t.IsActive);
    }
}
