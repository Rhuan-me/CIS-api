// CisApi.Infrastructure/Data/Config/TopicConfiguration.cs
using CisApi.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CisApi.Infrastructure.Data.Config;

public class TopicConfiguration : IEntityTypeConfiguration<Topic>
{
    public void Configure(EntityTypeBuilder<Topic> builder)
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
        
        builder.HasIndex(e => e.OwnedBy)
            .HasDatabaseName("IX_Topics_OwnedBy");
        
        builder.HasMany(e => e.Ideas)
            .WithOne(e => e.Topic)
            .HasForeignKey(e => e.TopicId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}