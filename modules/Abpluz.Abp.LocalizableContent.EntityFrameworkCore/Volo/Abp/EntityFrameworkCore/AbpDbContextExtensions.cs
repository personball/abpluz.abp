using Abpluz.Abp.LocalizableContents;
using Microsoft.EntityFrameworkCore;

namespace Volo.Abp.EntityFrameworkCore
{
    public static class AbpDbContextExtensions
    {
        public static void ConfigureLocalizableContentEntities(this ModelBuilder modelBuilder)
        {
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(IHasLocalizableContent).IsAssignableFrom(entityType.ClrType))
                {
                    modelBuilder.Entity(entityType.ClrType).Property(nameof(IHasLocalizableContent.CultureName))
                        .IsRequired()
                        .HasDefaultValue(false)
                        .HasColumnName(nameof(IHasLocalizableContent.CultureName))
                        .HasMaxLength(LocalizableContentConsts.CultureNameMaxLength);
                }
            }
        }
    }
}
