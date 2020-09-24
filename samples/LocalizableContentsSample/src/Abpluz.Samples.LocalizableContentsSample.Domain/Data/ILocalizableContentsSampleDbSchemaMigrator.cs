using System.Threading.Tasks;

namespace Abpluz.Samples.LocalizableContentsSample.Data
{
    public interface ILocalizableContentsSampleDbSchemaMigrator
    {
        Task MigrateAsync();
    }
}
