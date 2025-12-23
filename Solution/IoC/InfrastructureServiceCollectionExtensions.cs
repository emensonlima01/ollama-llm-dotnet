using Microsoft.Extensions.DependencyInjection;

namespace IoC;

public static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services
            .AddRepositories()
            .AddDomainServices();

        return services;
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        return services;
    }

    private static IServiceCollection AddDomainServices(this IServiceCollection services)
    {
        return services;
    }
}
