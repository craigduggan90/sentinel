using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sentinel.Data.Context.Converters;
using Sentinel.Domain.Entities;
using System.Diagnostics.CodeAnalysis;

namespace Sentinel.Data.Context.Configuration;

[ExcludeFromCodeCoverage]
public class AuditRecordConfiguration : IEntityTypeConfiguration<AuditRecord>
{
    public void Configure(EntityTypeBuilder<AuditRecord> builder)
    {
        builder.ToTable("audit_records");
        builder.HasKey(entity => entity.Id);

        // Deliberately not a foreign key - the audit trail has to outlive the rows it describes.
        builder.HasIndex(entity => new { entity.UserId, entity.Timestamp });

        builder.Property(entity => entity.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        builder.Property(entity => entity.UserId)
            .HasColumnName("user_id")
            .HasMaxLength(36);

        builder.Property(entity => entity.Timestamp)
            .HasColumnName("timestamp")
            .HasConversion<UtcDateTimeConverter>();

        builder.Property(entity => entity.Description)
            .HasColumnName("description")
            .HasMaxLength(1000);

        builder.Property(entity => entity.Data)
            .HasColumnName("data");
    }
}