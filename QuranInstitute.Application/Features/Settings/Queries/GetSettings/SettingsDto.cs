namespace QuranInstitute.Application.Features.Settings.Queries.GetSettings;

public class SettingsDto
{
    public Guid Id { get; set; }
    public string InstituteName { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? LogoUrl { get; set; }
    public string AbsentTemplate { get; set; } = string.Empty;
    public string DebtTemplate { get; set; } = string.Empty;
    public string GeneralTemplate { get; set; } = string.Empty;
}