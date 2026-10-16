using MediatR;
using Microsoft.EntityFrameworkCore;
using QuranInstitute.Application.Common.Interfaces;
using QuranInstitute.Application.Common.Models;
using QuranInstitute.Domain.Enums;

namespace QuranInstitute.Application.Features.Enrollments.Commands.ApproveEnrollment;

public class ApproveEnrollmentCommandHandler
    : IRequestHandler<ApproveEnrollmentCommand, Result>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _current;

    public ApproveEnrollmentCommandHandler(
        IApplicationDbContext db,
        ICurrentUserService current)
    {
        _db = db;
        _current = current;
    }

    public async Task<Result> Handle(ApproveEnrollmentCommand request, CancellationToken ct)
    {
        var e = await _db.Enrollments
            .Include(x => x.Class)
            .FirstOrDefaultAsync(x => x.Id == request.Id, ct);

        if (e is null)
            return Result.Failure("ثبت‌نام یافت نشد.");

        if (e.Status != EnrollmentStatus.Pending)
            return Result.Failure("این ثبت‌نام قبلاً بررسی شده است.");

        // بررسی ظرفیت کلاس
        var approvedCount = await _db.Enrollments
            .CountAsync(x => x.ClassId == e.ClassId
                          && x.Status == EnrollmentStatus.Approved, ct);

        if (e.Class.Capacity > 0 && approvedCount >= e.Class.Capacity)
            return Result.Failure("ظرفیت این کلاس پر است.");

        e.Status = EnrollmentStatus.Approved;
        e.ApprovedAt = DateTime.UtcNow;
        e.ApprovedBy = _current.UserId;

        await _db.SaveChangesAsync(ct);
        return Result.Success();
    }
}