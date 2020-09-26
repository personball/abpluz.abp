using Microsoft.EntityFrameworkCore;
using Pluz.Sample.DemoProducts;
using Volo.Abp;
using Volo.Abp.EntityFrameworkCore;

namespace Pluz.Sample.EntityFrameworkCore
{
    public static class SampleDbContextModelCreatingExtensions
    {
        public static void ConfigureSample(this ModelBuilder builder)
        {
            Check.NotNull(builder, nameof(builder));

            /* Configure your own tables/entities inside here */

            //builder.Entity<YourEntity>(b =>
            //{
            //    b.ToTable(SampleConsts.DbTablePrefix + "YourEntities", SampleConsts.DbSchema);
            //    b.ConfigureByConvention(); //auto configure for the base class props
            //    //...
            //});

            builder.Entity<DemoProduct>(opt =>
            {
                opt.HasMany(b => b.Entries).WithOne().HasForeignKey(e => e.Id);
            });
            builder.Entity<DemoProductLocalizableEntry>(opt =>
            {
                opt.HasKey(b => new { b.Id, b.CultureName });
            });
            
            builder.ConfigureLocalizableContentEntities();
        }
    }
}