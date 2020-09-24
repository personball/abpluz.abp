using Microsoft.EntityFrameworkCore;
using Volo.Abp;

namespace Abpluz.Samples.SwitchableIdentityClientsSample.EntityFrameworkCore
{
    public static class SwitchableIdentityClientsSampleDbContextModelCreatingExtensions
    {
        public static void ConfigureSwitchableIdentityClientsSample(this ModelBuilder builder)
        {
            Check.NotNull(builder, nameof(builder));

            /* Configure your own tables/entities inside here */

            //builder.Entity<YourEntity>(b =>
            //{
            //    b.ToTable(SwitchableIdentityClientsSampleConsts.DbTablePrefix + "YourEntities", SwitchableIdentityClientsSampleConsts.DbSchema);
            //    b.ConfigureByConvention(); //auto configure for the base class props
            //    //...
            //});
        }
    }
}