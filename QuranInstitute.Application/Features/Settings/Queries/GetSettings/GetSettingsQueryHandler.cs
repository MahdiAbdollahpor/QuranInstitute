using MediatR;
using Microsoft.EntityFrameworkCore;
using QuranInstitute.Application.Common.Interfaces;
using QuranInstitute.Application.Common.Models;
using QuranInstitute.Domain.Entities.System;

namespace QuranInstitute.Application.Features.Settings.Queries.GetSettings;

public class GetSettingsQueryHandler
    : IRequestHandler<GetSettingsQuery, Result<SettingsDto>>
{
    private readonly IApplicationDbContext _db;

    public GetSettingsQueryHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result<SettingsDto>> Handle(
        GetSettingsQuery request, CancellationToken ct)
    {
        var settings = await _db.AppSettings
            .AsNoTracking()
            .FirstOrDefaultAsync(ct);

        if (settings is null)
        {
            // ساخت پیش‌فرض اگر نبود
            settings = new AppSettings();
            _db.AppSettings.Add(settings);
            await _db.SaveChangesAsync(ct);
        }

        return Result<SettingsDto>.Success(new SettingsDto
        {
            Id = settings.Id,
            InstituteName = settings.InstituteName,
            Address = settings.Address,
            Phone = settings.Phone,
            Email = settings.Email,
            LogoUrl = settings.LogoUrl,
            AbsentTemplate = settings.AbsentTemplate,
            DebtTemplate = settings.DebtTemplate,
            GeneralTemplate = settings.GeneralTemplate
        });
    }
}