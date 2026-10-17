using MediatR;
using Microsoft.EntityFrameworkCore;
using QuranInstitute.Application.Common.Interfaces;
using QuranInstitute.Application.Common.Models;
using QuranInstitute.Domain.Entities.Financial;
using QuranInstitute.Domain.Enums;

namespace QuranInstitute.Application.Features.Payments.Commands.RecordManualPayment;

public class RecordManualPaymentCommandHandler
    : IRequestHandler<RecordManualPaymentCommand, Result<Guid>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _current;

    public RecordManualPaymentCommandHandler(
        IApplicationDbContext db,
        ICurrentUserService current)
    {
        _db = db;
        _current = current;
    }

    public async Task<Result<Guid>> Handle(
        RecordManualPaymentCommand request, CancellationToken ct)
    {
        var student = await _db.Students
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == request.StudentId, ct);

        if (student is null)
            return Result<Guid>.Failure("شاگرد یافت نشد.");

        if (!student.Active)
            return Result<Guid>.Failure("این شاگرد غیرفعال است.");

        var payment = new Payment
        {
            StudentId = request.StudentId,
            Amount = request.Amount,
            Date = request.Date.Date,
            Method = request.Method,
            Status = PaymentStatus.Verified,   // پرداخت دستی، مستقیم تأییدشده
            Note = request.Note?.Trim(),
            ReceiptNo = await GenerateReceiptNoAsync(ct),
            RecordedBy = _current.UserId
        };

        _db.Payments.Add(payment);
        await _db.SaveChangesAsync(ct);

        return Result<Guid>.Success(payment.Id);
    }

    /// <summary>
    /// تولید شماره رسید یکتا: RC-{Year}{Month}{Sequence}
    /// </summary>
    private async Task<string> GenerateReceiptNoAsync(CancellationToken ct)
    {
        var year = DateTime.Today.Year;
        var month = DateTime.Today.Month;

        var prefix = $"RC-{year}{month:00}-";

        var lastNo = await _db.Payments
            .Where(p => p.ReceiptNo.StartsWith(prefix))
            .OrderByDescending(p => p.ReceiptNo)
            .Select(p => p.ReceiptNo)
            .FirstOrDefaultAsync(ct);

        int seq = 1;
        if (!string.IsNullOrEmpty(lastNo))
        {
            var parts = lastNo.Split('-');
            if (parts.Length >= 3 && int.TryParse(parts[^1], out var n))
                seq = n + 1;
        }

        return $"{prefix}{seq:0000}";
    }
}