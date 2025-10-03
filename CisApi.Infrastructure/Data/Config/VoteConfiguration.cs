// CisApi.Infrastructure/Data/Config/VoteConfiguration.cs
using CisApi.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CisApi.Infrastructure.Data.Config;

public class VoteConfiguration : IEntityTypeConfiguration<Vote>
{
    public void Configure(EntityTypeBuilder<Vote> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.VotedBy)
            .HasMaxLength(256)
            .IsRequired();
        
        builder.HasIndex(e => new { e.IdeaId, e.VotedBy })
            .IsUnique()
            .HasDatabaseName("IX_Votes_IdeaId_VotedBy");
              
        builder.HasIndex(e => e.VotedBy)
            .HasDatabaseName("IX_Votes_VotedBy");
    }
}