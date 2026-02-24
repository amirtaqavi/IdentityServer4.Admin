
using Aerospike.Client;
using Microsoft.Extensions.Logging;
using Skoruba.IdentityServer4.Admin.AeroSpike.Repositories.Interfaces;

namespace Skoruba.IdentityServer4.Admin.AeroSpike.Repositories;

public class AeroSpikeRepository<T> : IAeroSpikeRepository<T> where T : class, new()
{
    private readonly IAerospikeClient _client;
    private readonly string _namespace;
    private readonly ILogger<AeroSpikeRepository<T>> _logger;

    public AeroSpikeRepository(
        IAerospikeClient client,
        ILogger<AeroSpikeRepository<T>> logger,
        AeroSpikeConfiguration configuration)
    {
        _client = client;
        _logger = logger;
        _namespace = configuration.Namespace;
    }

    public async Task<T?> GetByIdAsync(string id)
    {
        try
        {
            var key = new Key(_namespace, typeof(T).Name, id);
            var record = await Task.Run(() => _client.Get(null, key));

            if (record == null) return null;

            return MapRecordToEntity(record);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting entity by ID: {Id}", id);
            return null;
        }
    }

    public async Task<IEnumerable<T>> GetAllAsync(string setName, int limit = 100)
    {
        try
        {
            var statement = new Statement()
                .SetNamespace(_namespace)
                .SetSetName(setName);

            var records = await Task.Run(() => _client.Query(null, statement));
            var results = new List<T>();

            while (records.Next())
            {
                var record = records.Record;
                if (record != null)
                {
                    results.Add(MapRecordToEntity(record));
                }
            }

            return results;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all entities from set: {SetName}", setName);
            return Enumerable.Empty<T>();
        }
    }

    public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
    {
        // Note: AeroSpike doesn't support complex queries out of the box
        // This is a basic implementation - consider using secondary indexes for production
        try
        {
            var statement = new Statement()
                .SetNamespace(_namespace)
                .SetSetName(typeof(T).Name);

            var records = await Task.Run(() => _client.Query(null, statement));
            var results = new List<T>();

            while (records.Next())
            {
                var record = records.Record;
                if (record != null)
                {
                    var entity = MapRecordToEntity(record);
                    // Apply predicate in memory (not efficient for large datasets)
                    if (predicate.Compile()(entity))
                    {
                        results.Add(entity);
                    }
                }
            }

            return results;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error finding entities");
            return Enumerable.Empty<T>();
        }
    }

    public async Task<T> CreateAsync(T entity)
    {
        try
        {
            var key = GenerateKey(entity);
            var bins = MapEntityToBins(entity);

            var policy = new WritePolicy();
            policy.recordExistsAction = RecordExistsAction.CREATE_ONLY;

            await Task.Run(() => _client.Put(policy, key, bins));
            return entity;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating entity");
            throw;
        }
    }

    public async Task<bool> UpdateAsync(string id, T entity)
    {
        try
        {
            var key = new Key(_namespace, typeof(T).Name, id);
            var bins = MapEntityToBins(entity);

            var policy = new WritePolicy();
            policy.recordExistsAction = RecordExistsAction.UPDATE_ONLY;

            await Task.Run(() => _client.Put(policy, key, bins));
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating entity: {Id}", id);
            return false;
        }
    }

    public async Task<bool> DeleteAsync(string id)
    {
        try
        {
            var key = new Key(_namespace, typeof(T).Name, id);
            var result = await Task.Run(() => _client.Delete(null, key));
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting entity: {Id}", id);
            return false;
        }
    }

    public async Task<bool> ExistsAsync(string id)
    {
        try
        {
            var key = new Key(_namespace, typeof(T).Name, id);
            var exists = await Task.Run(() => _client.Exists(null, key));
            return exists;
        }
        catch
        {
            return false;
        }
    }

    private Key GenerateKey(T entity)
    {
        var id = typeof(T).GetProperty("Id")?.GetValue(entity)?.ToString()
            ?? Guid.NewGuid().ToString();
        return new Key(_namespace, typeof(T).Name, id);
    }

    private Bin[] MapEntityToBins(T entity)
    {
        var bins = new List<Bin>();
        var properties = typeof(T).GetProperties();

        foreach (var prop in properties)
        {
            var value = prop.GetValue(entity);
            if (value != null)
            {
                bins.Add(new Bin(prop.Name, value));
            }
        }

        return bins.ToArray();
    }

    private T MapRecordToEntity(Record record)
    {
        var entity = new T();
        var properties = typeof(T).GetProperties();

        foreach (var prop in properties)
        {
            if (record.bins.TryGetValue(prop.Name, out var value))
            {
                prop.SetValue(entity, Convert.ChangeType(value, prop.PropertyType));
            }
        }

        return entity;
    }
}