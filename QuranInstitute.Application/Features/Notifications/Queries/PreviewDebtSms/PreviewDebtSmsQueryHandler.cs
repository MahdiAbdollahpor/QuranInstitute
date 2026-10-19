using MediatR;
using Microsoft.EntityFrameworkCore;
using QuranInstitute.Application.Common.Interfaces;
using QuranInstitute.Application.Common.Models;
using QuranInstitute.Application.Features.Tuition.Services;
using QuranInstitute.Domain.Enums;
using QuranInstitute.Shared.Helpers;

namespace QuranInstitute.Application.Features.Notifications.Queries.PreviewDebtSms;

public class PreviewDebtSmsQueryHandler
    : IRequestHandler<PreviewDebtSmsQuery, Result<DebtSmsPreviewResult>>
{
    private readonly IApplicationDbContext _db;
    private readonly ITemplateService _template;
    private readonly ITuitionCalculator _calculator;

    public PreviewDebtSmsQueryHandler(
        IApplicationDbContext db,
        ITemplateService template,
        ITuitionCalculator calculator)
    {
        _db = db;
        _template = template;
        _calculator = calculator;
    }

    public async Task<Result<DebtSmsPreviewResult>> Handle(
        PreviewDebtSmsQuery request, CancellationToken ct)
    {
        var template = await _template.GetTemplateAsync("debt", ct);
        var settings = await _db.AppSettings.AsNoTracking().FirstOrDefaultAsync(ct);
        var instituteName = settings?.InstituteName ?? "موسسه قرآنی";

        var q = _db.Students
            .AsNoTracking()
            .Where(s => s.Active);

        if (request.ClassId.HasValue)
        {
            q = q.Where(s => s.Enrollments.Any(e =>
                e.ClassId == request.ClassId.Value &&
                e.Status == EnrollmentStatus.Approved));
        }

        var students = await q
            .Select(s => new
            {
                s.Id,
                s.Name,
                s.ParentName,
                s.ParentMobile
            })
            .ToListAsync(ct);

        var result = new DebtSmsPreviewResult { Template = template };

        foreach (var s in students)
        {
            var calc = await _calculator.CalculateAsync(s.Id, ct);
            if (calc.Balance <= 0) continue;

            var amountStr = ((long)calc.Balance).ToString("N0").ConvertToFa() + " تومان";

            var vars = new Dictionary<string, string>
            {
                { "student", s.Name },
                { "parent", s.ParentName },
                { "amount", amountStr },
                { "institute", instituteName }
            };

            result.Recipients.Add(new DebtRecipientPreview
            {
                StudentId = s.Id,
                StudentName = s.Name,
                ParentName = s.ParentName,
                Mobile = s.ParentMobile,
                Balance = calc.Balance,
                RenderedMessage = _template.Render(template, vars)
            });
        }

        return Result<DebtSmsPreviewResult>.Success(result);
    }
}