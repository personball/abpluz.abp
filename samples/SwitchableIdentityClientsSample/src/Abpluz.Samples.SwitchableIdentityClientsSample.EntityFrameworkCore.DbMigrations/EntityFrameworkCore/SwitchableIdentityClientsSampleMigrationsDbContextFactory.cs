using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Abpluz.Samples.SwitchableIdentityClientsSample.EntityFrameworkCore
{
    /* This class is needed for EF Core console commands
     * (like Add-Migration and Update-Database commands) */
    public class SwitchableIdentityClientsSampleMigrationsDbContextFactory : IDesignTimeDbContextFactory<SwitchableIdentityClientsSampleMigrationsDbContext>
    {
        public SwitchableIdentityClientsSampleMigrationsDbContext CreateDbContext(string[] args)
        {
            SwitchableIdentityClientsSampleEfCoreEntityExtensionMappings.Configure();

            var configuration = BuildConfiguration();

            var builder = new DbContextOptionsBuilder<SwitchableIdentityClientsSampleMigrationsDbContext>()
                .UseSqlServer(configuration.GetConnectionString("Default"));

            return new SwitchableIdentityClientsSampleMigrationsDbContext(builder.Options);
        }

        private static IConfigurationRoot BuildConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../Abpluz.Samples.SwitchableIdentityClientsSample.DbMigrator/"))
                .AddJsonFile("appsettings.json", optional: false);

            return builder.Build();
        }
    }
}
