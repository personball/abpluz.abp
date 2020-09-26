using System;
using System.Globalization;
using System.Linq.Expressions;
using Abpluz.Abp.LocalizableContents;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Pluz.Sample.DemoProducts;
using Pluz.Sample.Users;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.Modeling;
using Volo.Abp.Identity;
using Volo.Abp.Users.EntityFrameworkCore;

namespace Pluz.Sample.EntityFrameworkCore
{
    /* This is your actual DbContext used on runtime.
     * It includes only your entities.
     * It does not include entities of the used modules, because each module has already
     * its own DbContext class. If you want to share some database tables with the used modules,
     * just create a structure like done for AppUser.
     *
     * Don't use this DbContext for database migrations since it does not contain tables of the
     * used modules (as explained above). See SampleMigrationsDbContext for migrations.
     */
    [ConnectionStringName("Default")]
    public class SampleDbContext : AbpDbContext<SampleDbContext>
    {
        public DbSet<AppUser> Users { get; set; }
        protected virtual string CurrentCultureName => CultureInfo.CurrentCulture.Name;

        protected virtual bool IsCultureEntryFilterEnabled => DataFilter?.IsEnabled<IHasLocalizableContent>() ?? false;


        /* Add DbSet properties for your Aggregate Roots / Entities here.
         * Also map them inside SampleDbContextModelCreatingExtensions.ConfigureSample
         */

        public SampleDbContext(DbContextOptions<SampleDbContext> options)
            : base(options)
        {

        }
        public virtual DbSet<DemoProduct> DemoProducts { get; set; }

        public virtual DbSet<DemoProductLocalizableEntry> DemoProductLocalizableEntries { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            /* Configure the shared tables (with included modules) here */

            builder.Entity<AppUser>(b =>
            {
                b.ToTable(AbpIdentityDbProperties.DbTablePrefix + "Users"); //Sharing the same table "AbpUsers" with the IdentityUser

                b.ConfigureByConvention();
                b.ConfigureAbpUser();

                /* Configure mappings for your additional properties
                 * Also see the SampleEfCoreEntityExtensionMappings class
                 */
            });

            /* Configure your own tables/entities inside the ConfigureSample method */

            builder.ConfigureSample();
        }

        protected override bool ShouldFilterEntity<TEntity>(IMutableEntityType entityType)
        {
            if (typeof(IHasLocalizableContent).IsAssignableFrom(typeof(TEntity)))
            {
                return true;
            }

            return base.ShouldFilterEntity<TEntity>(entityType);
        }

        protected override Expression<Func<TEntity, bool>> CreateFilterExpression<TEntity>()
        {
            var expression= base.CreateFilterExpression<TEntity>();

            if (typeof(IHasLocalizableContent).IsAssignableFrom(typeof(TEntity)))
            {
                // HACK IHasCultureEntry 自动根据当前CultureInfo添加过滤器
                Expression<Func<TEntity, bool>> cultureFilter = e => !IsCultureEntryFilterEnabled || EF.Property<string>(e, nameof(IHasLocalizableContent.CultureName)) == CurrentCultureName; // 当心变量捕获
                expression = expression == null ? cultureFilter : CombineExpressions(expression, cultureFilter);
            }

            return expression;
        }
    }
}
