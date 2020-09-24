using System.Threading.Tasks;

namespace Abpluz.Samples.SwitchableIdentityClientsSample.Data
{
    public interface ISwitchableIdentityClientsSampleDbSchemaMigrator
    {
        Task MigrateAsync();
    }
}
