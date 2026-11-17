using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sentinel.Data.Context.Configuration.Helpers;
using Sentinel.Data.Context.Converters;
using Sentinel.Domain.Entities;
using System.Diagnostics.CodeAnalysis;

namespace Sentinel.Data.Context.Configuration;

[ExcludeFromCodeCoverage]
public class VerificationConfiguration : IEntityTypeConfiguration<Verification>
{
    public void Configure(EntityTypeBuilder<Verification> builder)
    {
        builder.ConfigureEntityBase().ToTable("verifications");

        // Supports the challenge lookup: a user's verifications, by outcome, at or above an age.
        builder.HasIndex(entity => new { entity.UserId, entity.Status, entity.Age });

        builder.Property(entity => entity.UserId)
            .HasColumnName("user_id")
            .HasMaxLength(36);

        builder.Property(entity => entity.Age)
            .HasColumnName("age");

        builder.Property(entity => entity.Status)
            .HasColumnName("status")
            .HasConversion<int>();

        builder.Property(entity => entity.Timestamp)
            .HasColumnName("timestamp")
            .HasConversion<UtcDateTimeConverter>();

        builder.HasOne(entity => entity.User)
            .WithMany(user => user.Verifications)
            .HasForeignKey(entity => entity.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}