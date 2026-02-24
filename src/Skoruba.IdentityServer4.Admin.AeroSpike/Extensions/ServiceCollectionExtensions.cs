using Aerospike.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Skoruba.IdentityServer4.Admin.AeroSpike.Configuration;
using Skoruba.IdentityServer4.Admin.AeroSpike.Repositories;
using Skoruba.IdentityServer4.Admin.AeroSpike.Services;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Services;

namespace Skoruba.IdentityServer4.Admin.AeroSpike.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAeroSpikeAdminServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Configure AeroSpike
        var aeroSpikeConfig = configuration
            .GetSection("AeroSpike")
            .Get<AeroSpikeConfiguration>() ?? new AeroSpikeConfiguration();

        services.AddSingleton(aeroSpikeConfig);

        // Register AeroSpike client
        services.AddSingleton<IAerospikeClient>(sp =>
        {
            var config = sp.GetRequiredService<AeroSpikeConfiguration>();
            var clientPolicy = new ClientPolicy
            {
                timeout = config.ConnectionTimeout,
                maxRetries = config.MaxRetries,
                user = config.UseAuth ? config.User : null,
                password = config.UseAuth ? config.Password : null
            };

            var hosts = new[] { new Host(config.Host, config.Port) };
            return new AerospikeClient(clientPolicy, hosts);
        });

        // Register repositories
        services.AddScoped(typeof(IAeroSpikeRepository<>), typeof(AeroSpikeRepository<>));

        // Register admin services
        services.AddScoped<IIdentityResourceService, AeroSpikeAdminService>();
        services.AddScoped<IApiResourceService, AeroSpikeApiResourceService>();
        services.AddScoped<IApiScopeService, AeroSpikeApiScopeService>();
        services.AddScoped<IClientService, AeroSpikeClientService>();
        services.AddScoped<IPersistedGrantService, AeroSpikePersistedGrantService>();

        return services;
    }

    public static IServiceCollection AddAeroSpikeIdentityAdminServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddAeroSpikeAdminServices(configuration);

        // Add identity-specific services
        services.AddScoped<IIdentityService, AeroSpikeIdentityService>();
        services.AddScoped<IRoleService, AeroSpikeRoleService>();

        return services;
    }
}