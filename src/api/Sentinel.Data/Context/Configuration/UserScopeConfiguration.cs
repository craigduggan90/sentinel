using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sentinel.Domain.Entities;
using System.Diagnostics.CodeAnalysis;

namespace Sentinel.Data.Context.Configuration;

[ExcludeFromCodeCoverage]
public class UserScopeConfiguration : IEntityTypeConfiguration<UserScope>
{
    public void Configure(EntityTypeBuilder<UserScope> builder)
    {
        builder.ToTable("user_scopes");
        builder.HasKey(entity => new { entity.UserId, entity.Scope });

        builder.Property(entity => entity.UserId)
            .HasColumnName("user_id")
            .HasMaxLength(36);

        builder.Property(entity => entity.Scope)
            .HasColumnName("scope")
            .HasMaxLength(100);

        // Scopes only make sense alongside their user - hide them with the user, and remove them with it.
        builder.HasQueryFilter(entity => entity.User.DateDeleted == null);

        builder.HasOne(entity => entity.User)
            .WithMany(user => user.Scopes)
            .HasForeignKey(entity => entity.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}