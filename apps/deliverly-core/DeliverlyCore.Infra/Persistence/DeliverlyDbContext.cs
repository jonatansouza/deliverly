using DeliverlyCore.Pricing.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DeliverlyCore.Infra.Persistence
{
    public class DeliverlyDbContext : DbContext
    {
        public DeliverlyDbContext(DbContextOptions<DeliverlyDbContext> options) : base(options) { }

        public DbSet<TariffTable> TariffTables => Set<TariffTable>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(DeliverlyDbContext).Assembly);
        }
    }
}
