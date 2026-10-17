using MediatR;
using Microsoft.EntityFrameworkCore;
using QuranInstitute.Application.Common.Interfaces;
using QuranInstitute.Application.Common.Models;
using QuranInstitute.Domain.Entities.Attendances;
using QuranInstitute.Domain.Enums;

namespace QuranInstitute.Application.Features.Attendances.Commands.MarkAllPresent;

public class MarkAllPresentCommandHandler
    : IRequestHandler<MarkAllPresentCommand, Result>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _current;

    public MarkAllPresentCommandHandler(
        IApplicationDbContext db,
        ICurrentUserService current)
    {
        _db = db;
        _current = current;
    }

    public async Task<Result> Handle(MarkAllPresentCommand request, CancellationToken ct)
    {
        var date = request.Date.Date;
        var userId = _current.UserId;

        var studentIds = await _db.Enrollments
            .Where(e => e.ClassId == request.ClassId
                     && e.Status == EnrollmentStatus.Approved)
            .Select(e => e.StudentId)
            .ToListAsync(ct);

        if (!studentIds.Any())
            return Result.Failure("کلاس شاگردی ندارد.");

        var existing = await _db.Attendances
            .Where(a => a.ClassId == request.ClassId && a.Date == date)
            .ToDictionaryAsync(a => a.StudentId, ct);

        foreach (var sid in studentIds)
        {
            if (existing.TryGetValue(sid, out var att))
            {
                att.Status = AttendanceStatus.Present;
                att.UpdatedAt = DateTime.UtcNow;
                att.UpdatedBy = userId;
            }
            else
            {
                _db.Attendances.Add(new Attendance
                {
                    ClassId = request.ClassId,
                    StudentId = sid,
                    Date = date,
                    Status = AttendanceStatus.Present,
                    RecordedBy = userId ?? Guid.Empty
                });
            }
        }

        await _db.SaveChangesAsync(ct);
        return Result.Success();
    }
}