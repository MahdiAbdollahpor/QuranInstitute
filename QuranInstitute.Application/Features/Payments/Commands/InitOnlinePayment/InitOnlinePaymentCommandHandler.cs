using MediatR;
using Microsoft.EntityFrameworkCore;
using Parbad;
using QuranInstitute.Application.Common.Interfaces;
using QuranInstitute.Application.Common.Models;
using QuranInstitute.Domain.Entities.Financial;
using QuranInstitute.Domain.Enums;

namespace QuranInstitute.Application.Features.Payments.Commands.InitOnlinePayment;

public class InitOnlinePaymentCommandHandler
    : IRequestHandler<InitOnlinePaymentCommand, Result<IPaymentRequestResult>>
{
    private readonly IApplicationDbContext _db;
    private readonly IOnlinePayment _onlinePayment;
    private readonly ICurrentUserService _current;

    public InitOnlinePaymentCommandHandler(
        IApplicationDbContext db,
        IOnlinePayment onlinePayment,
        ICurrentUserService current)
    {
        _db = db;
        _onlinePayment = onlinePayment;
        _current = current;
    }

    public async Task<Result<IPaymentRequestResult>> Handle(
        InitOnlinePaymentCommand request, CancellationToken ct)
    {
        var student = await _db.Students
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == request.StudentId, ct);

        if (student is null)
            return Result<IPaymentRequestResult>.Failure("شاگرد یافت نشد.");

        if (!student.Active)
            return Result<IPaymentRequestResult>.Failure("این شاگرد غیرفعال است.");

        var payment = new Payment
        {
            StudentId = request.StudentId,
            Amount = request.Amount,
            Date = DateTime.Today,
            Method = PaymentMethod.Online,
            Status = PaymentStatus.Pending,
            Note = "پرداخت آنلاین",
            ReceiptNo = await GenerateReceiptNoAsync(ct),
            RecordedBy = _current.UserId
        };

        _db.Payments.Add(payment);
        await _db.SaveChangesAsync(ct);

        var callbackUrl = $"{request.CallbackBaseUrl}?paymentId={payment.Id}";
        var trackingNumber = (int)(DateTime.UtcNow.Ticks % int.MaxValue);

        var result = await _onlinePayment.RequestAsync(invoice =>
        {
            invoice
                .SetTrackingNumber(trackingNumber)
                .SetAmount((long)request.Amount)
                .SetCallbackUrl(callbackUrl)
                .SetGateway("ParbadVirtual");
        });

        if (!result.IsSucceed)
            return Result<IPaymentRequestResult>.Failure(
                result.Message ?? "خطا در ارتباط با درگاه پرداخت.");

        payment.GatewayRef = result.TrackingNumber.ToString();
        await _db.SaveChangesAsync(ct);

        // ★ نتیجه کامل Parbad را برگردان
        return Result<IPaymentRequestResult>.Success(result);
    }

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