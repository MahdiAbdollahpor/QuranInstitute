using MediatR;
using Microsoft.EntityFrameworkCore;
using QuranInstitute.Application.Common.Interfaces;
using QuranInstitute.Application.Common.Models;
using QuranInstitute.Application.Features.Tuition.Services;
using QuranInstitute.Domain.Enums;

namespace QuranInstitute.Application.Features.UserPanel.Queries.GetMyBalance;

public class GetMyBalanceQueryHandler
    : IRequestHandler<GetMyBalanceQuery, Result<MyBalanceDto>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _current;
    private readonly ITuitionCalculator _calc;

    public GetMyBalanceQueryHandler(
        IApplicationDbContext db,
        ICurrentUserService current,
        ITuitionCalculator calc)
    {
        _db = db;
        _current = current;
        _calc = calc;
    }

    public async Task<Result<MyBalanceDto>> Handle(
        GetMyBalanceQuery request, CancellationToken ct)
    {
        var userId = _current.UserId;
        if (!userId.HasValue)
            return Result<MyBalanceDto>.Failure("کاربر شناسایی نشد.");

        var student = await _db.Students
            .AsNoTracking()
            .Where(s => s.Id == request.StudentId && s.UserId == userId.Value)
            .Select(s => new { s.Id, s.Name, s.MonthlyFee, s.StartDate })
            .FirstOrDefaultAsync(ct);

        if (student is null)
            return Result<MyBalanceDto>.Failure("شاگرد یافت نشد یا دسترسی ندارید.");

        var calc = await _calc.CalculateAsync(student.Id, ct);

        var payments = await _db.Payments
            .AsNoTracking()
            .Where(p => p.StudentId == student.Id && p.Status == PaymentStatus.Verified)
            .OrderByDescending(p => p.Date)
            .Take(100)
            .Select(p => new MyPaymentRowDto
            {
                Id = p.Id,
                Date = p.Date,
                Amount = p.Amount,
                MethodName = p.Method == PaymentMethod.Cash ? "نقدی"
                    : p.Method == PaymentMethod.Card ? "کارت به کارت"
                    : p.Method == PaymentMethod.Online ? "آنلاین"
                    : "سایر",
                ReceiptNo = p.ReceiptNo,
                Note = p.Note
            })
            .ToListAsync(ct);

        return Result<MyBalanceDto>.Success(new MyBalanceDto
        {
            StudentId = student.Id,
            StudentName = student.Name,
            MonthlyFee = student.MonthlyFee,
            StartDate = student.StartDate,
            MonthCount = calc.MonthCount,
            TotalOwed = calc.TotalOwed,
            TotalPaid = calc.TotalPaid,
            Balance = calc.Balance,
            Payments = payments
        });
    }
}