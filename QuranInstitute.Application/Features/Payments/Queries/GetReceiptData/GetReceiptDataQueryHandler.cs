using MediatR;
using Microsoft.EntityFrameworkCore;
using QuranInstitute.Application.Common.Interfaces;
using QuranInstitute.Application.Common.Models;
using QuranInstitute.Application.Features.Tuition.Services;
using QuranInstitute.Domain.Enums;

namespace QuranInstitute.Application.Features.Payments.Queries.GetReceiptData;

public class GetReceiptDataQueryHandler
    : IRequestHandler<GetReceiptDataQuery, Result<ReceiptDto>>
{
    private readonly IApplicationDbContext _db;
    private readonly ITuitionCalculator _calculator;

    public GetReceiptDataQueryHandler(
        IApplicationDbContext db,
        ITuitionCalculator calculator)
    {
        _db = db;
        _calculator = calculator;
    }

    public async Task<Result<ReceiptDto>> Handle(
        GetReceiptDataQuery request, CancellationToken ct)
    {
        var payment = await _db.Payments
            .AsNoTracking()
            .Where(p => p.Id == request.PaymentId)
            .Select(p => new
            {
                p.Id,
                p.ReceiptNo,
                p.Date,
                p.Amount,
                p.Method,
                p.Note,
                p.StudentId,
                StudentName = p.Student.Name,
                ParentName = p.Student.ParentName,
                ClassName = p.Student.Enrollments
                    .Where(e => e.Status == EnrollmentStatus.Approved)
                    .Select(e => e.Class.Name)
                    .FirstOrDefault()
            })
            .FirstOrDefaultAsync(ct);

        if (payment is null)
            return Result<ReceiptDto>.Failure("پرداخت یافت نشد.");

        var calc = await _calculator.CalculateAsync(payment.StudentId, ct);

        // نام موسسه از پیام‌ها/تنظیمات — برای سادگی، اینجا hard-code شده
        // در گام تنظیمات، از DB خوانده می‌شود
        const string instituteName = "موسسه قرآنی";

        return Result<ReceiptDto>.Success(new ReceiptDto
        {
            PaymentId = payment.Id,
            ReceiptNo = payment.ReceiptNo,
            Date = payment.Date,
            Amount = payment.Amount,
            Method = payment.Method,
            Note = payment.Note,
            StudentName = payment.StudentName,
            ParentName = payment.ParentName,
            ClassName = payment.ClassName ?? "—",
            TotalOwed = calc.TotalOwed,
            TotalPaid = calc.TotalPaid,
            Balance = calc.Balance,
            InstituteName = instituteName
        });
    }
}