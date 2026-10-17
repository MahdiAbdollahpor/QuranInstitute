using MediatR;
using Microsoft.EntityFrameworkCore;
using QuranInstitute.Application.Common.Interfaces;
using QuranInstitute.Application.Common.Models;
using QuranInstitute.Application.Features.Tuition.Services;
using QuranInstitute.Domain.Enums;

namespace QuranInstitute.Application.Features.Tuition.Queries.GetStudentBalance;

public class GetStudentBalanceQueryHandler
    : IRequestHandler<GetStudentBalanceQuery, Result<StudentBalanceDto>>
{
    private readonly IApplicationDbContext _db;
    private readonly ITuitionCalculator _calculator;

    public GetStudentBalanceQueryHandler(
        IApplicationDbContext db,
        ITuitionCalculator calculator)
    {
        _db = db;
        _calculator = calculator;
    }

    public async Task<Result<StudentBalanceDto>> Handle(
        GetStudentBalanceQuery request, CancellationToken ct)
    {
        var student = await _db.Students
            .AsNoTracking()
            .Where(s => s.Id == request.StudentId)
            .Select(s => new
            {
                s.Id,
                s.Name,
                s.ParentName,
                s.ParentMobile,
                s.MonthlyFee,
                s.StartDate,
                ClassId = s.Enrollments
                    .Where(e => e.Status == EnrollmentStatus.Approved)
                    .Select(e => (Guid?)e.ClassId)
                    .FirstOrDefault(),
                ClassName = s.Enrollments
                    .Where(e => e.Status == EnrollmentStatus.Approved)
                    .Select(e => e.Class.Name)
                    .FirstOrDefault()
            })
            .FirstOrDefaultAsync(ct);

        if (student is null)
            return Result<StudentBalanceDto>.Failure("شاگرد یافت نشد.");

        var calc = await _calculator.CalculateAsync(student.Id, ct);

        return Result<StudentBalanceDto>.Success(new StudentBalanceDto
        {
            StudentId = student.Id,
            StudentName = student.Name,
            ParentName = student.ParentName,
            ParentMobile = student.ParentMobile,
            ClassId = student.ClassId,
            ClassName = student.ClassName ?? "—",
            MonthlyFee = student.MonthlyFee,
            StartDate = student.StartDate,
            MonthCount = calc.MonthCount,
            TotalOwed = calc.TotalOwed,
            TotalPaid = calc.TotalPaid,
            Balance = calc.Balance
        });
    }
}