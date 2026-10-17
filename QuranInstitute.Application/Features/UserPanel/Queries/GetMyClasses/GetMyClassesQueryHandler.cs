using MediatR;
using Microsoft.EntityFrameworkCore;
using QuranInstitute.Application.Common.Interfaces;
using QuranInstitute.Application.Common.Models;
using QuranInstitute.Domain.Enums;

namespace QuranInstitute.Application.Features.UserPanel.Queries.GetMyClasses;

public class GetMyClassesQueryHandler
    : IRequestHandler<GetMyClassesQuery, Result<List<MyClassDto>>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _current;

    public GetMyClassesQueryHandler(
        IApplicationDbContext db,
        ICurrentUserService current)
    {
        _db = db;
        _current = current;
    }

    public async Task<Result<List<MyClassDto>>> Handle(
        GetMyClassesQuery request, CancellationToken ct)
    {
        var userId = _current.UserId;
        if (!userId.HasValue)
            return Result<List<MyClassDto>>.Failure("کاربر شناسایی نشد.");

        // شاگردان کاربر
        var myStudentIds = await _db.Students
            .Where(s => s.UserId == userId.Value && s.Active)
            .Select(s => s.Id)
            .ToListAsync(ct);

        if (request.StudentId.HasValue)
        {
            if (!myStudentIds.Contains(request.StudentId.Value))
                return Result<List<MyClassDto>>.Failure("دسترسی ندارید.");
            myStudentIds = new List<Guid> { request.StudentId.Value };
        }

        if (!myStudentIds.Any())
            return Result<List<MyClassDto>>.Success(new List<MyClassDto>());

        var enrollments = await _db.Enrollments
            .AsNoTracking()
            .Where(e => myStudentIds.Contains(e.StudentId)
                     && e.Status == EnrollmentStatus.Approved)
            .Select(e => new
            {
                e.Id,
                e.StudentId,
                StudentName = e.Student.Name,
                e.ClassId,
                ClassName = e.Class.Name,
                TeacherName = e.Class.TeacherName,
                e.Class.ClassTime,
                e.Class.DaysCsv,
                e.Class.Type,
                e.Class.DefaultOnlineLink,
                e.ApprovedAt,
                e.RequestedAt
            })
            .ToListAsync(ct);

        // آمار حضور
        var classIds = enrollments.Select(e => e.ClassId).Distinct().ToList();

        var attendanceStats = await _db.Attendances
            .Where(a => myStudentIds.Contains(a.StudentId)
                     && classIds.Contains(a.ClassId))
            .GroupBy(a => new { a.StudentId, a.ClassId })
            .Select(g => new
            {
                g.Key.StudentId,
                g.Key.ClassId,
                Present = g.Count(x => x.Status == AttendanceStatus.Present),
                Absent = g.Count(x => x.Status == AttendanceStatus.Absent)
            })
            .ToListAsync(ct);

        var result = enrollments.Select(e =>
        {
            var stat = attendanceStats.FirstOrDefault(
                s => s.StudentId == e.StudentId && s.ClassId == e.ClassId);

            var days = string.IsNullOrWhiteSpace(e.DaysCsv)
                ? Array.Empty<int>()
                : e.DaysCsv.Split(',', StringSplitOptions.RemoveEmptyEntries)
                          .Select(int.Parse).ToArray();

            return new MyClassDto
            {
                ClassId = e.ClassId,
                ClassName = e.ClassName,
                TeacherName = e.TeacherName,
                ClassTime = e.ClassTime,
                Days = days,
                Type = e.Type,
                OnlineLink = e.DefaultOnlineLink,
                StudentName = e.StudentName,
                EnrollmentStatus = "تأییدشده",
                EnrolledAt = e.ApprovedAt ?? e.RequestedAt,
                TotalSessions = (stat?.Present ?? 0) + (stat?.Absent ?? 0),
                PresentCount = stat?.Present ?? 0,
                AbsentCount = stat?.Absent ?? 0
            };
        }).ToList();

        return Result<List<MyClassDto>>.Success(result);
    }
}