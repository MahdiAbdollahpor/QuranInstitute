using Microsoft.EntityFrameworkCore;
using QuranInstitute.Application.Common.Interfaces;

namespace QuranInstitute.Infrastructure.Services;

public class TemplateService : ITemplateService
{
    private readonly IApplicationDbContext _db;

    public TemplateService(IApplicationDbContext db) => _db = db;

    public string Render(string template, Dictionary<string, string> variables)
    {
        if (string.IsNullOrWhiteSpace(template))
            return string.Empty;

        var result = template;
        foreach (var kv in variables)
        {
            result = result.Replace($"{{{kv.Key}}}", kv.Value ?? "");
        }
        return result;
    }

    public async Task<string> GetTemplateAsync(string key, CancellationToken ct = default)
    {
        var settings = await _db.AppSettings
            .AsNoTracking()
            .FirstOrDefaultAsync(ct);

        if (settings is null) return string.Empty;

        return key switch
        {
            "absent" => settings.AbsentTemplate,
            "debt" => settings.DebtTemplate,
            "general" => settings.GeneralTemplate,
            _ => string.Empty
        };
    }
}