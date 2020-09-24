using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Abpluz.Samples.LocalizableContentsSample.EntityFrameworkCore
{
    /* This class is needed for EF Core console commands
     * (like Add-Migration and Update-Database commands) */
    public class LocalizableContentsSampleMigrationsDbContextFactory : IDesignTimeDbContextFactory<LocalizableContentsSampleMigrationsDbContext>
    {
        public LocalizableContentsSampleMigrationsDbContext CreateDbContext(string[] args)
        {
            LocalizableContentsSampleEfCoreEntityExtensionMappings.Configure();

            var configuration = BuildConfiguration();

            var builder = new DbContextOptionsBuilder<LocalizableContentsSampleMigrationsDbContext>()
                .UseSqlServer(configuration.GetConnectionString("Default"));

            return new LocalizableContentsSampleMigrationsDbContext(builder.Options);
        }

        private static IConfigurationRoot BuildConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../Abpluz.Samples.LocalizableContentsSample.DbMigrator/"))
                .AddJsonFile("appsettings.json", optional: false);

            return builder.Build();
        }
    }
}
