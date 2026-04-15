using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;

namespace Hephaestus.Features.HeadHunterClient;

public class HeadHunterClient(
    IOptions<HeadHunterClientSettings> options,
    HttpClient httpClient,
    ILogger<HeadHunterClient> logger) : IHeadHunterClient
{
    public async Task<HhVacanciesResponse?> SearchVacanciesAsync(string text, int page = 0, int perPage = 10, CancellationToken ct = default)
    {
        try
        {
            var requestUrl = $"vacancies?text={Uri.EscapeDataString(text)}&per_page={perPage}&page={page}";
            
            logger.LogInformation("Поиск вакансий: {Text}", text);
            var response = await httpClient.GetAsync(options.Value.BaseUrl + requestUrl, ct);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync(ct);
            
            var jsonSerializerOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var result = JsonSerializer.Deserialize<HhVacanciesResponse>(json, jsonSerializerOptions);
            
            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Ошибка поиска вакансий: {Text}", text);
            return null;
        }
    }
    
    public async Task<HhVacancyDetail?> GetVacancyAsync(string vacancyId, CancellationToken ct = default)
    {
        try
        {
            var requestUrl = $"vacancies/{vacancyId}";
        
            logger.LogInformation("Получение деталей вакансии: {VacancyId}", vacancyId);
        
            var response = await httpClient.GetAsync(options.Value.BaseUrl + requestUrl, ct);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync(ct);
        
            var jsonSerializerOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var result = JsonSerializer.Deserialize<HhVacancyDetail>(json, jsonSerializerOptions);
        
            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Ошибка получения деталей вакансии: {VacancyId}", vacancyId);
            return null;
        }
    }
}


public record HhVacanciesResponse(
    [property: JsonPropertyName("items")] List<HhVacancy> Items,
    [property: JsonPropertyName("found")] int Found,
    [property: JsonPropertyName("pages")] int Pages,
    [property: JsonPropertyName("page")] int Page,
    [property: JsonPropertyName("per_page")] int PerPage,
    [property: JsonPropertyName("alternate_url")] string? AlternateUrl
);

public record HhVacancy(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("premium")] bool Premium,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("area")] HhArea Area,
    [property: JsonPropertyName("salary")] HhSalary? Salary,
    [property: JsonPropertyName("salary_range")] HhSalaryRange? SalaryRange,
    [property: JsonPropertyName("type")] HhVacancyType Type,
    [property: JsonPropertyName("address")] HhAddress? Address,
    [property: JsonPropertyName("published_at")] string PublishedAt,
    [property: JsonPropertyName("created_at")] string CreatedAt,     
    [property: JsonPropertyName("archived")] bool Archived,
    [property: JsonPropertyName("alternate_url")] string AlternateUrl,
    [property: JsonPropertyName("url")] string Url,
    [property: JsonPropertyName("employer")] HhEmployer Employer,
    [property: JsonPropertyName("snippet")] HhSnippet Snippet,
    [property: JsonPropertyName("experience")] HhExperience Experience,
    [property: JsonPropertyName("employment_form")] HhEmploymentForm? EmploymentForm,
    [property: JsonPropertyName("work_format")] List<HhIdName>? WorkFormat,
    [property: JsonPropertyName("professional_roles")] List<HhIdName> ProfessionalRoles
);

public record HhArea(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("url")] string Url
);

public record HhSalary(
    [property: JsonPropertyName("from")] int? From,
    [property: JsonPropertyName("to")] int? To,
    [property: JsonPropertyName("currency")] string Currency,
    [property: JsonPropertyName("gross")] bool Gross
);

public record HhSalaryRange(
    [property: JsonPropertyName("from")] int? From,
    [property: JsonPropertyName("to")] int? To,
    [property: JsonPropertyName("currency")] string Currency,
    [property: JsonPropertyName("gross")] bool Gross,
    [property: JsonPropertyName("mode")] HhIdName Mode,
    [property: JsonPropertyName("frequency")] HhIdName Frequency
);

public record HhVacancyType(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("name")] string Name
);

public record HhAddress(
    [property: JsonPropertyName("city")] string? City,
    [property: JsonPropertyName("street")] string? Street,
    [property: JsonPropertyName("building")] string? Building,
    [property: JsonPropertyName("lat")] double? Lat,
    [property: JsonPropertyName("lng")] double? Lng,
    [property: JsonPropertyName("raw")] string? Raw,
    [property: JsonPropertyName("metro")] HhMetro? Metro,
    [property: JsonPropertyName("metro_stations")] List<HhMetro> MetroStations
);

public record HhMetro(
    [property: JsonPropertyName("station_id")] string StationId,
    [property: JsonPropertyName("station_name")] string StationName,
    [property: JsonPropertyName("line_id")] string LineId,
    [property: JsonPropertyName("line_name")] string LineName,
    [property: JsonPropertyName("lat")] double Lat,
    [property: JsonPropertyName("lng")] double Lng
);

public record HhEmployer(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("url")] string Url,
    [property: JsonPropertyName("alternate_url")] string AlternateUrl,
    [property: JsonPropertyName("logo_urls")] HhLogoUrls? LogoUrls,
    [property: JsonPropertyName("trusted")] bool Trusted,
    [property: JsonPropertyName("accredited_it_employer")] bool AccreditedItEmployer
);

public record HhLogoUrls(
    [property: JsonPropertyName("original")] string? Original,
    [property: JsonPropertyName("90")] string? Size90,
    [property: JsonPropertyName("240")] string? Size240
);

public record HhSnippet(
    [property: JsonPropertyName("requirement")] string? Requirement,
    [property: JsonPropertyName("responsibility")] string? Responsibility
);

public record HhExperience(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("name")] string Name
);

public record HhEmploymentForm(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("name")] string Name
);

public record HhIdName(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("name")] string Name
);

public record HhVacancyDetail(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("premium")] bool Premium,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("description")] string? Description,
    [property: JsonPropertyName("key_skills")] List<HhKeySkill>? KeySkills,
    [property: JsonPropertyName("area")] HhArea Area,
    [property: JsonPropertyName("salary")] HhSalary? Salary,
    [property: JsonPropertyName("salary_range")] HhSalaryRange? SalaryRange,
    [property: JsonPropertyName("type")] HhVacancyType Type,
    [property: JsonPropertyName("address")] HhAddress? Address,
    [property: JsonPropertyName("published_at")] string PublishedAt,
    [property: JsonPropertyName("created_at")] string CreatedAt,
    [property: JsonPropertyName("archived")] bool Archived,
    [property: JsonPropertyName("alternate_url")] string AlternateUrl,
    [property: JsonPropertyName("url")] string Url,
    [property: JsonPropertyName("employer")] HhEmployerDetail? Employer,
    [property: JsonPropertyName("experience")] HhExperience Experience,
    [property: JsonPropertyName("employment_form")] HhEmploymentForm? EmploymentForm,
    [property: JsonPropertyName("work_format")] List<HhIdName>? WorkFormat,
    [property: JsonPropertyName("professional_roles")] List<HhIdName> ProfessionalRoles,
    [property: JsonPropertyName("contacts")] HhContacts? Contacts,
    [property: JsonPropertyName("test")] HhTest? Test,
    [property: JsonPropertyName("response_letter_required")] bool ResponseLetterRequired,
    [property: JsonPropertyName("accept_temporary")] bool AcceptTemporary,
    [property: JsonPropertyName("internship")] bool Internship,
    [property: JsonPropertyName("adv_response_url")] string? AdvResponseUrl,
    [property: JsonPropertyName("is_adv_vacancy")] bool IsAdvVacancy,
    [property: JsonPropertyName("branded_description")] string? BrandedDescription
);

public record HhKeySkill(
    [property: JsonPropertyName("name")] string Name
);

public record HhEmployerDetail(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("url")] string Url,
    [property: JsonPropertyName("alternate_url")] string AlternateUrl,
    [property: JsonPropertyName("logo_urls")] HhLogoUrls? LogoUrls,
    [property: JsonPropertyName("trusted")] bool Trusted,
    [property: JsonPropertyName("accredited_it_employer")] bool AccreditedItEmployer,
    [property: JsonPropertyName("description")] string? Description,
    [property: JsonPropertyName("site_url")] string? SiteUrl,
    [property: JsonPropertyName("area")] HhArea? Area,
    [property: JsonPropertyName("industries")] List<HhIdName>? Industries
);

public record HhContacts(
    [property: JsonPropertyName("name")] string? Name,
    [property: JsonPropertyName("email")] string? Email,
    [property: JsonPropertyName("phones")] List<HhPhone>? Phones
);

public record HhPhone(
    [property: JsonPropertyName("city")] string? City,
    [property: JsonPropertyName("country")] string? Country,
    [property: JsonPropertyName("number")] string? Number,
    [property: JsonPropertyName("comment")] string? Comment
);

public record HhTest(
    [property: JsonPropertyName("id")] string? Id,
    [property: JsonPropertyName("required")] bool Required
);