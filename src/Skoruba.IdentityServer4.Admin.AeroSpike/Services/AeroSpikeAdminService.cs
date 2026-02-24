using Skoruba.IdentityServer4.Admin.BusinessLogic.Services;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Dtos.Configuration;
using Skoruba.IdentityServer4.Admin.AeroSpike.Repositories;

namespace Skoruba.IdentityServer4.Admin.AeroSpike.Services;

public class AeroSpikeAdminService : IIdentityResourceService
{
    private readonly IAeroSpikeRepository<IdentityResourceEntity> _repository;
    private readonly ILogger<AeroSpikeAdminService> _logger;

    public AeroSpikeAdminService(
        IAeroSpikeRepository<IdentityResourceEntity> repository,
        ILogger<AeroSpikeAdminService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<IdentityResourcesDto> GetIdentityResourcesAsync(string search, int page = 1, int pageSize = 10)
    {
        try
        {
            var allResources = await _repository.GetAllAsync("identityresources");
            var resources = allResources
                .Where(r => string.IsNullOrEmpty(search) ||
                           r.ToDto().Name.Contains(search, StringComparison.OrdinalIgnoreCase))
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(r => r.ToDto())
                .ToList();

            return new IdentityResourcesDto
            {
                IdentityResources = resources,
                TotalCount = allResources.Count(),
                PageSize = pageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting identity resources");
            return new IdentityResourcesDto();
        }
    }

    public async Task<IdentityResourceDto> GetIdentityResourceAsync(int identityResourceId)
    {
        var entity = await _repository.GetByIdAsync(identityResourceId.ToString());
        return entity?.ToDto() ?? new IdentityResourceDto();
    }

    public async Task<bool> CanInsertIdentityResourceAsync(IdentityResourceDto identityResource)
    {
        var resources = await _repository.FindAsync(r =>
            r.ToDto().Name == identityResource.Name);
        return !resources.Any();
    }

    public async Task<int> SaveIdentityResourceAsync(IdentityResourceDto identityResource)
    {
        var entity = IdentityResourceEntity.FromDto(identityResource);
        await _repository.CreateAsync(entity);
        return int.Parse(entity.Id);
    }

    public async Task UpdateIdentityResourceAsync(IdentityResourceDto identityResource)
    {
        await _repository.UpdateAsync(identityResource.Id.ToString(),
            IdentityResourceEntity.FromDto(identityResource));
    }

    public async Task DeleteIdentityResourceAsync(IdentityResourceDto identityResource)
    {
        await _repository.DeleteAsync(identityResource.Id.ToString());
    }

    public async Task<bool> CanInsertIdentityResourcePropertyAsync(IdentityResourcePropertyDto identityResourcePropertyDto)
    {
        // Implement property validation
        return true;
    }

    public async Task SaveIdentityResourcePropertyAsync(IdentityResourcePropertyDto identityResourceProperty)
    {
        // Implement property saving
        await Task.CompletedTask;
    }

    public async Task DeleteIdentityResourcePropertyAsync(IdentityResourcePropertyDto identityResourceProperty)
    {
        // Implement property deletion
        await Task.CompletedTask;
    }

    public async Task<IdentityResourcePropertiesDto> GetIdentityResourcePropertiesAsync(int identityResourceId, int page = 1, int pageSize = 10)
    {
        // Implement getting properties
        return new IdentityResourcePropertiesDto();
    }

    public async Task<IdentityResourcePropertyDto> GetIdentityResourcePropertyAsync(int identityResourcePropertyId)
    {
        // Implement getting property
        return new IdentityResourcePropertyDto();
    }

    // Implement other required methods...
}