using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace DeliverlyCore.Infra.Persistence
{
    // Used by EF Core CLI tools (dotnet ef migrations add, dotnet ef database update)
    public class DeliverlyDbContextFactory : IDesignTimeDbContextFactory<DeliverlyDbContext>
    {
        public DeliverlyDbContext CreateDbContext(string[] args)
        {
            var options = new DbContextOptionsBuilder<DeliverlyDbContext>()
                .UseNpgsql(
                    Environment.GetEnvironmentVariable("DB_CONNECTION"))
                .Options;

            return new DeliverlyDbContext(options);
        }
    }
}
