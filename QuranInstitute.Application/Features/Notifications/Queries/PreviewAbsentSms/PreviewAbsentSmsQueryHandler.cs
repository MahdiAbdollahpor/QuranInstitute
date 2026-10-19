using MediatR;
using Microsoft.EntityFrameworkCore;
using QuranInstitute.Application.Common.Interfaces;
using QuranInstitute.Application.Common.Models;
using QuranInstitute.Domain.Enums;
using QuranInstitute.Shared.Helpers;

namespace QuranInstitute.Application.Features.Notifications.Queries.PreviewAbsentSms;

public class PreviewAbsentSmsQueryHandler
    : IRequestHandler<PreviewAbsentSmsQuery, Result<SmsPreviewResult>>
{
    private readonly IApplicationDbContext _db;
    private readonly ITemplateService _template;

    public PreviewAbsentSmsQueryHandler(
        IApplicationDbContext db,
        ITemplateService template)
    {
        _db = db;
        _template = template;
    }

    public async Task<Result<SmsPreviewResult>> Handle(
        PreviewAbsentSmsQuery request, CancellationToken ct)
    {
        var date = request.Date.Date;

        var template = await _template.GetTemplateAsync("absent", ct);
        var settings = await _db.AppSettings.AsNoTracking().FirstOrDefaultAsync(ct);
        var instituteName = settings?.InstituteName ?? "موسسه قرآنی";

        var q = _db.Attendances
            .AsNoTracking()
            .Where(a => a.Date == date && a.Status == AttendanceStatus.Absent);

        if (request.ClassId.HasValue)
            q = q.Where(a => a.ClassId == request.ClassId.Value);

        var raw = await q
            .OrderBy(a => a.Class.Name)
            .ThenBy(a => a.Student.Name)
            .Select(a => new
            {
                a.StudentId,
                StudentName = a.Student.Name,
                ParentName = a.Student.ParentName,
                Mobile = a.Student.ParentMobile,
                ClassName = a.Class.Name
            })
            .ToListAsync(ct);

        var result = new SmsPreviewResult { Template = template };

        foreach (var r in raw)
        {
            var vars = new Dictionary<string, string>
            {
                { "student", r.StudentName },
                { "parent", r.ParentName },
                { "class", r.ClassName },
                { "date", date.GetShamsiDateFromGregorian(time: false) },
                { "institute", instituteName }
            };

            result.Recipients.Add(new SmsRecipientPreview
            {
                StudentId = r.StudentId,
                StudentName = r.StudentName,
                ParentName = r.ParentName,
                Mobile = r.Mobile,
                RenderedMessage = _template.Render(template, vars)
            });
        }

        return Result<SmsPreviewResult>.Success(result);
    }
}