using Application.Abstractions;
using Infrastructure.ChatProviders;
using Infrastructure.ChatProviders.Ollama;
using System;
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

        services.Configure<ChatProviderOptions>(configuration.GetSection("ChatProvider"));
        services.AddHttpClient<IChatProvider, OllamaChatProvider>((sp, client) =>
        {
            var options = sp.GetRequiredService<IOptions<ChatProviderOptions>>().Value;
            if (!string.Equals(options.Provider, "Ollama", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException($"Unsupported chat provider '{options.Provider}'.");
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
