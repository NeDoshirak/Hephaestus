using Duende.AccessTokenManagement;
using Duende.IdentityModel.Client;
using Hephaestus.Features.HeadHunterClient;

namespace Hephaestus.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApiClients(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        ConfigureClients(services, configuration);
        
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
        
        services.AddClientCredentialsTokenManagement()
            .AddClient("headhunter", client =>
            {
                client.TokenEndpoint = new Uri(settings.TokenEndpoint);
                client.ClientId = ClientId.Parse(settings.ClientId);
                client.ClientSecret = ClientSecret.Parse(settings.ClientSecret);
                
                client.Parameters = new Parameters
                {
                    { "grant_type", settings.GrantType }
                };
            });   
        
        services.AddHttpClient<IHeadHunterClient, HeadHunterClient>(client => 
                client.DefaultRequestHeaders.Add("User-Agent", "SkillSpace/0.1 (ujhjvjn@yandex.ru)"))
            .AddClientCredentialsTokenHandler(ClientCredentialsClientName.Parse("headhunter"));
    }
}