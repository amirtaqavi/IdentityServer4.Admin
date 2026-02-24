using Aerospike.Client;

namespace Skoruba.IdentityServer4.Admin.AeroSpike.Repositories.Interfaces
{
    public interface IAerospikeSchemaProvider
    {
        List<string> GetBinNames();
        string GetSetName();
        string GetNamespace();
        List<(string IndexName, string BinName, IndexType Type)> GetIndexes();
    }
}