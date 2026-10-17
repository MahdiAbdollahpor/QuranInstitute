using MediatR;
using Microsoft.EntityFrameworkCore;
using QuranInstitute.Application.Common.Interfaces;
using QuranInstitute.Application.Common.Models;
using QuranInstitute.Domain.Enums;

namespace QuranInstitute.Application.Features.UserPanel.Queries.GetAvailableClasses;

public class GetAvailableClassesQueryHandler
    : IRequestHandler<GetAvailableClassesQuery, Result<List<AvailableClassDto>>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _current;

    public GetAvailableClassesQueryHandler(
        IApplicationDbContext db,
        ICurrentUserService current)
    {
        _db = db;
        _current = current;
    }

    public async Task<Result<List<AvailableClassDto>>> Handle(
        GetAvailableClassesQuery request, CancellationToken ct)
    {
        var userId = _current.UserId;
        if (!userId.HasValue)
            return Result<List<AvailableClassDto>>.Failure("کاربر شناسایی نشد.");

        var studentIds = await _db.Students
            .Where(s => s.UserId == userId.Value && s.Active)
            .Select(s => s.Id)
            .ToListAsync(ct);

        if (request.StudentId.HasValue)
        {
            if (!studentIds.Contains(request.StudentId.Value))
                return Result<List<AvailableClassDto>>.Failure("دسترسی ندارید.");
            studentIds = new List<Guid> { request.StudentId.Value };
        }

        var enrolledClassIds = await _db.Enrollments
            .Where(e => studentIds.Contains(e.StudentId)
                     && e.Status == EnrollmentStatus.Approved)
            .Select(e => e.ClassId)
            .Distinct()
            .ToListAsync(ct);

        var classes = await _db.Classes
            .AsNoTracking()
            .Where(c => c.Active)
            .Select(c => new
            {
                c.Id,
                c.Name,
                c.TeacherName,
                c.ClassTime,
                c.DaysCsv,
                c.Type,
                c.Capacity,
                StudentCount = c.Enrollments.Count(e =>
                    e.Status == EnrollmentStatus.Approved)
            })
            .ToListAsync(ct);

        var result = classes.Select(c => new AvailableClassDto
        {
            ClassId = c.Id,
            ClassName = c.Name,
            TeacherName = c.TeacherName,
            ClassTime = c.ClassTime,
            Days = string.IsNullOrWhiteSpace(c.DaysCsv)
                ? Array.Empty<int>()
                : c.DaysCsv.Split(',', StringSplitOptions.RemoveEmptyEntries)
                          .Select(int.Parse).ToArray(),
            Type = c.Type,
            Capacity = c.Capacity,
            StudentCount = c.StudentCount,
            IsEnrolled = enrolledClassIds.Contains(c.Id)
        }).ToList();

        return Result<List<AvailableClassDto>>.Success(result);
    }
}