using MediatR;
using Microsoft.EntityFrameworkCore;
using QuranInstitute.Application.Common.Interfaces;
using QuranInstitute.Application.Common.Models;
using QuranInstitute.Domain.Enums;

namespace QuranInstitute.Application.Features.Enrollments.Commands.CancelEnrollment;

public class CancelEnrollmentCommandHandler
    : IRequestHandler<CancelEnrollmentCommand, Result>
{
    private readonly IApplicationDbContext _db;

    public CancelEnrollmentCommandHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result> Handle(CancelEnrollmentCommand request, CancellationToken ct)
    {
        var e = await _db.Enrollments
            .FirstOrDefaultAsync(x => x.Id == request.Id, ct);

        if (e is null)
            return Result.Failure("ثبت‌نام یافت نشد.");

        if (e.Status == EnrollmentStatus.Canceled)
            return Result.Failure("این ثبت‌نام قبلاً لغو شده است.");

        e.Status = EnrollmentStatus.Canceled;
        await _db.SaveChangesAsync(ct);
        return Result.Success();
    }
}