using System.Threading.Tasks;

namespace Pluz.Sample.Data;

public interface ISampleDbSchemaMigrator
{
    Task MigrateAsync();
}
