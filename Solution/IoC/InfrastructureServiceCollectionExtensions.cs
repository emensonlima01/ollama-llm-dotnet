using Infrastructure.ChatProviders;
using Infrastructure.ChatProviders.Ollama;
using System;
using Domain.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace IoC;

public static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddRepositories()
            .AddDomainServices();

        services.Configure<ChatProviderSettings>(configuration.GetSection("ChatProvider"));
        services.AddHttpClient<IChatModelProvider, OllamaChatModelProvider>((sp, client) =>
        {
            var options = sp.GetRequiredService<IOptions<ChatProviderSettings>>().Value;
            if (!string.Equals(options.ActiveProvider, "Ollama", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException($"Unsupported chat provider '{options.ActiveProvider}'.");
            }

            client.BaseAddress = new Uri(options.Ollama.BaseUrl);
        });

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
