using MediatR;
using Microsoft.EntityFrameworkCore;
using Parbad;
using QuranInstitute.Application.Common.Interfaces;
using QuranInstitute.Application.Common.Models;
using QuranInstitute.Domain.Enums;

namespace QuranInstitute.Application.Features.Payments.Commands.VerifyOnlinePayment;

public class VerifyOnlinePaymentCommandHandler
    : IRequestHandler<VerifyOnlinePaymentCommand, Result<Guid>>
{
    private readonly IApplicationDbContext _db;
    private readonly IOnlinePayment _onlinePayment;

    public VerifyOnlinePaymentCommandHandler(
        IApplicationDbContext db,
        IOnlinePayment onlinePayment)
    {
        _db = db;
        _onlinePayment = onlinePayment;
    }

    public async Task<Result<Guid>> Handle(
        VerifyOnlinePaymentCommand request, CancellationToken ct)
    {
        var payment = await _db.Payments
            .FirstOrDefaultAsync(p => p.Id == request.PaymentId, ct);

        if (payment is null)
            return Result<Guid>.Failure("پرداخت یافت نشد.");

        if (payment.Status == PaymentStatus.Verified)
            return Result<Guid>.Failure("این پرداخت قبلاً تأیید شده است.");

        // دریافت اطلاعات از Parbad
        var invoice = await _onlinePayment.FetchAsync();

        if (invoice.Status != PaymentFetchResultStatus.ReadyForVerifying)
        {
            payment.Status = PaymentStatus.Rejected;
            await _db.SaveChangesAsync(ct);
            return Result<Guid>.Failure("پرداخت معتبر نیست یا قبلاً پردازش شده است.");
        }

        // تأیید
        var verifyResult = await _onlinePayment.VerifyAsync(invoice);

        if (!verifyResult.IsSucceed || verifyResult.Status != PaymentVerifyResultStatus.Succeed)
        {
            payment.Status = PaymentStatus.Rejected;
            await _db.SaveChangesAsync(ct);
            return Result<Guid>.Failure(verifyResult.Message ?? "تأیید پرداخت ناموفق بود.");
        }

        // به‌روزرسانی Payment
        payment.Status = PaymentStatus.Verified;
        payment.GatewayRef = verifyResult.TransactionCode;
        payment.Note = $"پرداخت آنلاین - کد پیگیری: {verifyResult.TransactionCode}";

        await _db.SaveChangesAsync(ct);



        return Result<Guid>.Success(payment.Id);
    }
}