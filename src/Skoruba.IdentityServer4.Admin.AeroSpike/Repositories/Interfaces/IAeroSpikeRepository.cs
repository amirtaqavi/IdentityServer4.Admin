using System.Linq.Expressions;
using Aerospike.Client;

namespace Skoruba.IdentityServer4.Admin.AeroSpike.Repositories.Interfaces;


public interface IAeroSpikeRepository<T> where T : class
{
    Task<T?> GetByIdAsync(string id);
    Task<IEnumerable<T>> GetAllAsync(string setName, int limit = 100);
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
    Task<T> CreateAsync(T entity);
    Task<bool> UpdateAsync(string id, T entity);
    Task<bool> DeleteAsync(string id);
    Task<bool> ExistsAsync(string id);
}

