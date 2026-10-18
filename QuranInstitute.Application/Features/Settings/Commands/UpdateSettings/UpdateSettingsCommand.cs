using MediatR;
using QuranInstitute.Application.Common.Models;

namespace QuranInstitute.Application.Features.Settings.Commands.UpdateSettings;

public class UpdateSettingsCommand : IRequest<Result>
{
    public string InstituteName { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? LogoUrl { get; set; }
    public string AbsentTemplate { get; set; } = string.Empty;
    public string DebtTemplate { get; set; } = string.Empty;
    public string GeneralTemplate { get; set; } = string.Empty;
}