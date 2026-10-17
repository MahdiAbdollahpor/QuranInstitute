using MediatR;
using Microsoft.EntityFrameworkCore;
using QuranInstitute.Application.Common.Interfaces;
using QuranInstitute.Application.Common.Models;
using QuranInstitute.Domain.Enums;

namespace QuranInstitute.Application.Features.UserPanel.Queries.GetMyProfile;

public class GetMyProfileQueryHandler
    : IRequestHandler<GetMyProfileQuery, Result<MyProfileDto>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _current;

    public GetMyProfileQueryHandler(
        IApplicationDbContext db,
        ICurrentUserService current)
    {
        _db = db;
        _current = current;
    }

    public async Task<Result<MyProfileDto>> Handle(
        GetMyProfileQuery request, CancellationToken ct)
    {
        var userId = _current.UserId;
        if (!userId.HasValue)
            return Result<MyProfileDto>.Failure("کاربر شناسایی نشد.");

        var user = await _db.Users
            .AsNoTracking()
            .Where(u => u.Id == userId.Value)
            .Select(u => new
            {
                u.Id,
                u.FullName,
                u.UserName,
                Mobile = u.PhoneNumber
            })
            .FirstOrDefaultAsync(ct);

        if (user is null)
            return Result<MyProfileDto>.Failure("کاربر یافت نشد.");

        var students = await _db.Students
            .AsNoTracking()
            .Where(s => s.UserId == userId.Value)
            .Select(s => new MyStudentInfo
            {
                StudentId = s.Id,
                Name = s.Name,
                ParentName = s.ParentName,
                ClassName = s.Enrollments
                    .Where(e => e.Status == EnrollmentStatus.Approved)
                    .Select(e => e.Class.Name)
                    .FirstOrDefault() ?? "—",
                MonthlyFee = s.MonthlyFee,
                StartDate = s.StartDate
            })
            .ToListAsync(ct);

        return Result<MyProfileDto>.Success(new MyProfileDto
        {
            UserId = user.Id,
            FullName = user.FullName,
            Mobile = user.Mobile ?? user.UserName ?? "",
            Students = students
        });
    }
}