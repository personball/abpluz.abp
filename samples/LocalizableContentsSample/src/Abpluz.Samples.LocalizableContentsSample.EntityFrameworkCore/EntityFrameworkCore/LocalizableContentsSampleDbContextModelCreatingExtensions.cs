using Microsoft.EntityFrameworkCore;
using Volo.Abp;

namespace Abpluz.Samples.LocalizableContentsSample.EntityFrameworkCore
{
    public static class LocalizableContentsSampleDbContextModelCreatingExtensions
    {
        public static void ConfigureLocalizableContentsSample(this ModelBuilder builder)
        {
            Check.NotNull(builder, nameof(builder));

            /* Configure your own tables/entities inside here */

            //builder.Entity<YourEntity>(b =>
            //{
            //    b.ToTable(LocalizableContentsSampleConsts.DbTablePrefix + "YourEntities", LocalizableContentsSampleConsts.DbSchema);
            //    b.ConfigureByConvention(); //auto configure for the base class props
            //    //...
            //});
        }
    }
}