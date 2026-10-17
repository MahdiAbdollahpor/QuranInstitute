using MediatR;
using Microsoft.EntityFrameworkCore;
using QuranInstitute.Application.Common.Interfaces;
using QuranInstitute.Application.Common.Models;
using QuranInstitute.Domain.Enums;

namespace QuranInstitute.Application.Features.Attendances.Queries.GetAttendanceSummary;

public class GetAttendanceSummaryQueryHandler
    : IRequestHandler<GetAttendanceSummaryQuery, Result<AttendanceSummaryDto>>
{
    private readonly IApplicationDbContext _db;

    public GetAttendanceSummaryQueryHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result<AttendanceSummaryDto>> Handle(
        GetAttendanceSummaryQuery request, CancellationToken ct)
    {
        var date = request.Date.Date;

        var totalStudents = await _db.Enrollments
            .CountAsync(e => e.ClassId == request.ClassId
                          && e.Status == EnrollmentStatus.Approved, ct);

        var records = await _db.Attendances
            .AsNoTracking()
            .Where(a => a.ClassId == request.ClassId && a.Date == date)
            .GroupBy(a => a.Status)
            .Select(g => new { Status = g.Key, Count = g.Count() })
            .ToListAsync(ct);

        var summary = new AttendanceSummaryDto
        {
            TotalStudents = totalStudents,
            Present = records.FirstOrDefault(r => r.Status == AttendanceStatus.Present)?.Count ?? 0,
            Absent = records.FirstOrDefault(r => r.Status == AttendanceStatus.Absent)?.Count ?? 0,
            Late = records.FirstOrDefault(r => r.Status == AttendanceStatus.Late)?.Count ?? 0,
            Excused = records.FirstOrDefault(r => r.Status == AttendanceStatus.Excused)?.Count ?? 0,
            AlreadyRecorded = records.Any()
        };

        summary.NotRecorded = totalStudents
            - summary.Present - summary.Absent - summary.Late - summary.Excused;

        return Result<AttendanceSummaryDto>.Success(summary);
    }
}