using MediatR;
using Microsoft.EntityFrameworkCore;
using QuranInstitute.Application.Common.Interfaces;
using QuranInstitute.Domain.Enums;
using QuranInstitute.Shared.Helpers;

namespace QuranInstitute.Application.Features.Reports.Queries.ExportData;

public class ExportStudentsCsvQueryHandler
    : IRequestHandler<ExportStudentsCsvQuery, string>
{
    private readonly IApplicationDbContext _db;

    public ExportStudentsCsvQueryHandler(IApplicationDbContext db) => _db = db;

    public async Task<string> Handle(ExportStudentsCsvQuery request, CancellationToken ct)
    {
        var q = _db.Students.AsNoTracking().AsQueryable();

        if (request.ActiveOnly)
            q = q.Where(s => s.Active);

        var students = await q
            .OrderBy(s => s.Name)
            .Select(s => new
            {
                s.Name,
                s.ParentName,
                s.ParentMobile,
                s.MonthlyFee,
                s.StartDate,
                s.Active,
                ClassName = s.Enrollments
                    .Where(e => e.Status == EnrollmentStatus.Approved)
                    .Select(e => e.Class.Name)
                    .FirstOrDefault()
            })
            .ToListAsync(ct);

        var csv = new System.Text.StringBuilder();
        csv.AppendLine("نام شاگرد,نام ولی,موبایل,کلاس,شهریه ماهانه,تاریخ شروع,وضعیت");

        foreach (var s in students)
        {
            csv.AppendLine(string.Join(",",
                Escape(s.Name),
                Escape(s.ParentName),
                Escape("=\"" + s.ParentMobile + "\""),    // برای Excel
                Escape(s.ClassName ?? ""),
                s.MonthlyFee.ToString("F0"),
                Escape(s.StartDate.GetShamsiDateFromGregorian(time: false)),
                s.Active ? "فعال" : "غیرفعال"));
        }

        return csv.ToString();
    }

    private static string Escape(string? s)
    {
        if (string.IsNullOrEmpty(s)) return "";
        if (s.Contains(',') || s.Contains('"') || s.Contains('\n'))
            return "\"" + s.Replace("\"", "\"\"") + "\"";
        return s;
    }
}