using MediatR;
using Microsoft.EntityFrameworkCore;
using QuranInstitute.Application.Common.Interfaces;
using QuranInstitute.Domain.Enums;
using QuranInstitute.Shared.Helpers;

namespace QuranInstitute.Application.Features.Reports.Queries.ExportData;

public class ExportPaymentsCsvQueryHandler
    : IRequestHandler<ExportPaymentsCsvQuery, string>
{
    private readonly IApplicationDbContext _db;

    public ExportPaymentsCsvQueryHandler(IApplicationDbContext db) => _db = db;

    public async Task<string> Handle(ExportPaymentsCsvQuery request, CancellationToken ct)
    {
        var q = _db.Payments.AsNoTracking();

        if (request.FromDate.HasValue)
            q = q.Where(p => p.Date >= request.FromDate.Value.Date);

        if (request.ToDate.HasValue)
            q = q.Where(p => p.Date <= request.ToDate.Value.Date);

        var payments = await q
            .OrderByDescending(p => p.Date)
            .Select(p => new
            {
                p.ReceiptNo,
                p.Date,
                StudentName = p.Student.Name,
                p.Amount,
                p.Method,
                p.Status,
                p.Note
            })
            .ToListAsync(ct);

        var methodNames = new Dictionary<PaymentMethod, string>
        {
            { PaymentMethod.Cash, "نقدی" },
            { PaymentMethod.Card, "کارت به کارت" },
            { PaymentMethod.Online, "آنلاین" },
            { PaymentMethod.Other, "سایر" }
        };

        var statusNames = new Dictionary<PaymentStatus, string>
        {
            { PaymentStatus.Pending, "در انتظار" },
            { PaymentStatus.Verified, "تأییدشده" },
            { PaymentStatus.Rejected, "ردشده" }
        };

        var csv = new System.Text.StringBuilder();
        csv.AppendLine("شماره رسید,تاریخ,شاگرد,مبلغ,روش,وضعیت,توضیح");

        foreach (var p in payments)
        {
            csv.AppendLine(string.Join(",",
                Escape(p.ReceiptNo),
                Escape(p.Date.GetShamsiDateFromGregorian(time: false)),
                Escape(p.StudentName),
                p.Amount.ToString("F0"),
                Escape(methodNames[p.Method]),
                Escape(statusNames[p.Status]),
                Escape(p.Note ?? "")));
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