using MediatR;
using Microsoft.EntityFrameworkCore;
using QuranInstitute.Application.Common.Interfaces;
using QuranInstitute.Application.Common.Models;
using QuranInstitute.Domain.Entities.System;

namespace QuranInstitute.Application.Features.Settings.Commands.UpdateSettings;

public class UpdateSettingsCommandHandler
    : IRequestHandler<UpdateSettingsCommand, Result>
{
    private readonly IApplicationDbContext _db;

    public UpdateSettingsCommandHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result> Handle(
        UpdateSettingsCommand request, CancellationToken ct)
    {
        var settings = await _db.AppSettings.FirstOrDefaultAsync(ct);

        if (settings is null)
        {
            settings = new AppSettings();
            _db.AppSettings.Add(settings);
        }

        settings.InstituteName = request.InstituteName.Trim();
        settings.Address = request.Address?.Trim();
        settings.Phone = request.Phone?.Trim();
        settings.Email = request.Email?.Trim();
        settings.LogoUrl = request.LogoUrl?.Trim();
        settings.AbsentTemplate = request.AbsentTemplate;
        settings.DebtTemplate = request.DebtTemplate;
        settings.GeneralTemplate = request.GeneralTemplate;

        await _db.SaveChangesAsync(ct);
        return Result.Success();
    }
}