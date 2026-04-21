using System.Net.Http.Headers;
using Hephaestus.Features.HeadHunterClient;
using Hephaestus.Features.VacancySaver;

namespace Hephaestus.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApiClients(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        ConfigureClients(services, configuration);
        
        services.AddScoped<IVacancySaver, VacancySaver>();
        
        return services;
    }

    private static void ConfigureClients(IServiceCollection services, IConfiguration configuration)
    {
        ConfigureHeadHunterClient(services, configuration);
    }

    private static void ConfigureHeadHunterClient(IServiceCollection services, IConfiguration configuration)
    {
        var settings = configuration.GetSection("HeadHunter").Get<HeadHunterClientSettings>();
        
        if (settings == null)
        {
            throw new InvalidOperationException("HeadHunter settings are not configured properly");
        }
        
        services.Configure<HeadHunterClientSettings>(configuration.GetSection("HeadHunter"));

        services.AddHttpClient<IHeadHunterClient, HeadHunterClient>(client =>
        {
            client.DefaultRequestHeaders.Add("HH-User-Agent", "SkillSpace/0.1 (ujhjvjn@yandex.ru)");
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", configuration.GetSection("HeadHunter:AccessToken").Value);
        });
    }
}