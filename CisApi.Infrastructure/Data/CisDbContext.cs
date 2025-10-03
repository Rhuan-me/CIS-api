// CisApi.Infrastructure/Data/CisDbContext.cs
using System.Reflection;
using CisApi.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace CisApi.Infrastructure.Data;

public class CisDbContext : DbContext
{
    public CisDbContext(DbContextOptions<CisDbContext> options) : base(options) { }

    // Mapeia nossas classes de entidade para tabelas no banco de dados
    public DbSet<Topic> Topics { get; set; } = null!;
    public DbSet<Idea> Ideas { get; set; } = null!;
    public DbSet<Vote> Votes { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Aplica todas as configurações de entidade definidas neste assembly
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}