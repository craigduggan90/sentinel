using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sentinel.Data.Context.Configuration.Helpers;
using Sentinel.Domain.Entities;
using System.Diagnostics.CodeAnalysis;

namespace Sentinel.Data.Context.Configuration;

[ExcludeFromCodeCoverage]
public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ConfigureEntityBase().ToTable("users");
        builder.HasIndex(entity => entity.RemoteId).IsUnique();

        // Deleted users keep their row, so uniqueness only applies to live ones - this lets someone come back through
        // a different IdP with the same email address.
        builder.HasIndex(entity => entity.Email)
            .IsUnique()
            .HasFilter("date_deleted IS NULL");

        builder.HasIndex(entity => entity.Type);

        builder.Property(entity => entity.RemoteId)
            .HasColumnName("remote_id")
            .HasMaxLength(255);

        builder.Property(entity => entity.Email)
            .HasColumnName("email")
            .HasMaxLength(1000);

        builder.Property(entity => entity.Type)
            .HasColumnName("type")
            .HasConversion<int>();
    }
}