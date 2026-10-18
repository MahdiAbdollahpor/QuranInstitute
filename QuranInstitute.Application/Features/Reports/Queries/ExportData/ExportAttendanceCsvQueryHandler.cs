using MediatR;
using Microsoft.EntityFrameworkCore;
using QuranInstitute.Application.Common.Interfaces;
using QuranInstitute.Domain.Enums;
using QuranInstitute.Shared.Helpers;

namespace QuranInstitute.Application.Features.Reports.Queries.ExportData;

public class ExportAttendanceCsvQueryHandler
    : IRequestHandler<ExportAttendanceCsvQuery, string>
{
    private readonly IApplicationDbContext _db;

    public ExportAttendanceCsvQueryHandler(IApplicationDbContext db) => _db = db;

    public async Task<string> Handle(ExportAttendanceCsvQuery request, CancellationToken ct)
    {
        var fromDate = MyDateTime.GetGregorianDateFromShamsiMonthAndYear(
            request.JalaliYear, request.JalaliMonth);
        var toDate = fromDate.AddMonths(1);

        var records = await _db.Attendances
            .AsNoTracking()
            .Where(a => a.Date >= fromDate && a.Date < toDate)
            .OrderBy(a => a.Date)
            .Select(a => new
            {
                a.Date,
                ClassName = a.Class.Name,
                StudentName = a.Student.Name,
                a.Status,
                a.Note
            })
            .ToListAsync(ct);

        var statusNames = new Dictionary<AttendanceStatus, string>
        {
            { AttendanceStatus.Present, "حاضر" },
            { AttendanceStatus.Absent, "غایب" },
            { AttendanceStatus.Late, "تأخیر" },
            { AttendanceStatus.Excused, "موجه" }
        };

        var csv = new System.Text.StringBuilder();
        csv.AppendLine("تاریخ,کلاس,شاگرد,وضعیت,توضیح");

        foreach (var r in records)
        {
            csv.AppendLine(string.Join(",",
                Escape(r.Date.GetShamsiDateFromGregorian(time: false)),
                Escape(r.ClassName),
                Escape(r.StudentName),
                Escape(statusNames[r.Status]),
                Escape(r.Note ?? "")));
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