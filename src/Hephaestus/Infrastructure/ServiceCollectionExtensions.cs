using System.Net.Http.Headers;
using Hephaestus.Features.GroqClient;
using Hephaestus.Features.HeadHunterClient;
using Hephaestus.Features.OpenAiClients;
using Hephaestus.Features.OpenRouterClient;
using Hephaestus.Features.ProfessionsManagement.Interfaces;
using Hephaestus.Features.ProfessionsManagement.Services;
using Hephaestus.Features.SkillManagement;
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
        services.AddScoped<ISkillNormalizationService, SkillNormalizationService>();
        services.AddScoped<ISkillImportService, SkillImportService>();
        services.AddScoped<ISkillVerificationService, SkillVerificationService>();
        services.AddScoped<IProfessionService, ProfessionService>();

        return services;
    }

    private static void ConfigureClients(IServiceCollection services, IConfiguration configuration)
    {
        ConfigureHeadHunterClient(services, configuration);
        ConfigureAiClients(services, configuration);
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

    private static void ConfigureAiClients(IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<OpenRouterOptions>(configuration.GetSection(OpenRouterOptions.SectionName));
        services.Configure<GroqOptions>(configuration.GetSection(GroqOptions.SectionName));

        services.AddHttpClient<IOpenAiHttpClient, OpenAiHttpClient>(client =>
        {
            client.Timeout = TimeSpan.FromSeconds(30);
        });

        services.AddScoped<IOpenRouterClient, OpenRouterClient>();
        services.AddScoped<IGroqClient, GroqClient>();
    }
}