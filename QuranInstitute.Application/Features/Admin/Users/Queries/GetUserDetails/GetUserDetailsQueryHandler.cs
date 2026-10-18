using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using QuranInstitute.Application.Common.Interfaces;
using QuranInstitute.Application.Common.Models;
using QuranInstitute.Domain.Entities.Users;
using QuranInstitute.Domain.Enums;

namespace QuranInstitute.Application.Features.Admin.Users.Queries.GetUserDetails;

public class GetUserDetailsQueryHandler
    : IRequestHandler<GetUserDetailsQuery, Result<UserDetailsDto>>
{
    private readonly IApplicationDbContext _db;
    private readonly UserManager<AppUser> _userMgr;

    public GetUserDetailsQueryHandler(
        IApplicationDbContext db,
        UserManager<AppUser> userMgr)
    {
        _db = db;
        _userMgr = userMgr;
    }

    public async Task<Result<UserDetailsDto>> Handle(
        GetUserDetailsQuery request, CancellationToken ct)
    {
        var user = await _userMgr.FindByIdAsync(request.UserId.ToString());
        if (user is null)
            return Result<UserDetailsDto>.Failure("کاربر یافت نشد.");

        var roles = await _userMgr.GetRolesAsync(user);

        var linkedStudents = await _db.Students
            .AsNoTracking()
            .Where(s => s.UserId == request.UserId)
            .Select(s => new LinkedStudentInfo
            {
                StudentId = s.Id,
                Name = s.Name,
                ParentName = s.ParentName,
                ClassName = s.Enrollments
                    .Where(e => e.Status == EnrollmentStatus.Approved)
                    .Select(e => e.Class.Name)
                    .FirstOrDefault() ?? "—",
                Active = s.Active
            })
            .ToListAsync(ct);

        return Result<UserDetailsDto>.Success(new UserDetailsDto
        {
            Id = user.Id,
            FullName = user.FullName,
            Mobile = user.PhoneNumber ?? user.UserName ?? "",
            NationalCode = user.NationalCode,
            Email = user.Email,
            AvatarUrl = user.AvatarUrl,
            IsActive = user.IsActive,
            CreatedAt = user.CreatedAt,
            LastLoginAt = user.LastLoginAt,
            Roles = roles.ToList(),
            LinkedStudents = linkedStudents
        });
    }
}