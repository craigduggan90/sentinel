using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sentinel.Data.Context.Converters;
using Sentinel.Domain.Entities.Abstract;
using System.Diagnostics.CodeAnalysis;

namespace Sentinel.Data.Context.Configuration.Helpers;

/// <summary>Extension methods used to configure entities derived from <see cref="PageableEntityBase"/>.</summary>
[ExcludeFromCodeCoverage]
public static class PageableEntityBaseConfigurationExtensions
{
    public static EntityTypeBuilder<T> ConfigureEntityBase<T>(this EntityTypeBuilder<T> builder)
        where T : PageableEntityBase
    {
        // Honour soft-deletion
        builder.HasQueryFilter(entity => entity.DateDeleted == null);

        // Add simple indexes
        builder.HasKey(entity => entity.Id);
        builder.HasIndex(entity => entity.Cursor).IsUnique();
        builder.HasIndex(entity => entity.DateDeleted);
        builder.HasIndex(entity => entity.DateCreated);
        builder.HasIndex(entity => entity.DateModified);

        // Configure common properties
        builder.Property(entity => entity.Id)
            .HasColumnName("id")
            .HasMaxLength(36);

        builder.Property(entity => entity.Cursor)
            .HasColumnName("cursor");

        builder.Property(entity => entity.DateCreated)
            .HasColumnName("date_created")
            .HasConversion<UtcDateTimeConverter>();

        builder.Property(entity => entity.DateModified)
            .HasColumnName("date_modified")
            .HasConversion<UtcDateTimeConverter>();

        builder.Property(entity => entity.DateDeleted)
            .HasColumnName("date_deleted")
            .HasConversion<UtcDateTimeConverter>();

        // Ignore the dirty flag
        builder.Ignore(entity => entity.IsDirty);

        return builder;
    }
}