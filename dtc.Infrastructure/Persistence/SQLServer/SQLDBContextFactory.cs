using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace dtc.Infrastructure.Pesistence.SQLServer
{
    public class SQLDBContextFactory : IDesignTimeDbContextFactory<SQLDBContext>
    {
        public SQLDBContext CreateDbContext(string[] args)
        {
            var currentDirectory = Directory.GetCurrentDirectory();
            var basePath = new[]
            {
                Path.Combine(currentDirectory, "dtc.API"),
                Path.GetFullPath(Path.Combine(currentDirectory, "..", "dtc.API")),
                currentDirectory
            }.First(Directory.Exists);

            var configuration = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: false)
                .AddJsonFile("appsettings.Development.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<SQLDBContext>();
            optionsBuilder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));

            return new SQLDBContext(optionsBuilder.Options);
        }
    }
}
