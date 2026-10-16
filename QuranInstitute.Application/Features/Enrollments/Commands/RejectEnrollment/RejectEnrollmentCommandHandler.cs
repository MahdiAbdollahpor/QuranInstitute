using MediatR;
using Microsoft.EntityFrameworkCore;
using QuranInstitute.Application.Common.Interfaces;
using QuranInstitute.Application.Common.Models;
using QuranInstitute.Domain.Enums;

namespace QuranInstitute.Application.Features.Enrollments.Commands.RejectEnrollment;

public class RejectEnrollmentCommandHandler
    : IRequestHandler<RejectEnrollmentCommand, Result>
{
    private readonly IApplicationDbContext _db;

    public RejectEnrollmentCommandHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result> Handle(RejectEnrollmentCommand request, CancellationToken ct)
    {
        var e = await _db.Enrollments
            .FirstOrDefaultAsync(x => x.Id == request.Id, ct);

        if (e is null)
            return Result.Failure("ثبت‌نام یافت نشد.");

        if (e.Status != EnrollmentStatus.Pending)
            return Result.Failure("این ثبت‌نام قبلاً بررسی شده است.");

        e.Status = EnrollmentStatus.Rejected;
        e.Note = string.IsNullOrWhiteSpace(request.Reason)
            ? e.Note
            : request.Reason.Trim();

        await _db.SaveChangesAsync(ct);
        return Result.Success();
    }
}