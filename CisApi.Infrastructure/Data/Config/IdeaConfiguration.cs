// CisApi.Infrastructure/Data/Config/IdeaConfiguration.cs
using CisApi.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CisApi.Infrastructure.Data.Config;

public class IdeaConfiguration : IEntityTypeConfiguration<Idea>
{
    public void Configure(EntityTypeBuilder<Idea> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Title)
            .HasMaxLength(255)
            .IsRequired();
              
        builder.Property(e => e.Description)
            .HasMaxLength(1000);
              
        builder.Property(e => e.OwnedBy)
            .HasMaxLength(256)
            .IsRequired();
              
        builder.Property(e => e.VoteCount)
            .HasDefaultValue(0);
        
        builder.HasIndex(e => e.TopicId)
            .HasDatabaseName("IX_Ideas_TopicId");
              
        builder.HasIndex(e => e.OwnedBy)
            .HasDatabaseName("IX_Ideas_OwnedBy");
        
        builder.HasMany(e => e.Votes)
            .WithOne(e => e.Idea)
            .HasForeignKey(e => e.IdeaId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}