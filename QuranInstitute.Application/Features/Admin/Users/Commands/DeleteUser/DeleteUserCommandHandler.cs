using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using QuranInstitute.Application.Common.Interfaces;
using QuranInstitute.Application.Common.Models;
using QuranInstitute.Domain.Entities.Users;

namespace QuranInstitute.Application.Features.Admin.Users.Commands.DeleteUser;

public class DeleteUserCommandHandler
    : IRequestHandler<DeleteUserCommand, Result>
{
    private readonly IApplicationDbContext _db;
    private readonly UserManager<AppUser> _userMgr;
    private readonly ICurrentUserService _current;

    public DeleteUserCommandHandler(
        IApplicationDbContext db,
        UserManager<AppUser> userMgr,
        ICurrentUserService current)
    {
        _db = db;
        _userMgr = userMgr;
        _current = current;
    }

    public async Task<Result> Handle(
        DeleteUserCommand request, CancellationToken ct)
    {
        if (_current.UserId == request.UserId)
            return Result.Failure("نمی‌توانید حساب خودتان را حذف کنید.");

        var user = await _userMgr.FindByIdAsync(request.UserId.ToString());
        if (user is null)
            return Result.Failure("کاربر یافت نشد.");

        // بررسی نقش Admin — اگر آخرین ادمین است، جلوگیری
        var roles = await _userMgr.GetRolesAsync(user);
        if (roles.Contains("Admin"))
        {
            var admins = await _userMgr.GetUsersInRoleAsync("Admin");
            if (admins.Count(a => a.IsActive) <= 1)
                return Result.Failure("نمی‌توانید آخرین ادمین فعال را حذف کنید.");
        }

        // قطع ارتباط با شاگردان (SetNull)
        var students = await _db.Students
            .Where(s => s.UserId == request.UserId)
            .ToListAsync(ct);

        foreach (var s in students)
            s.UserId = null;

        await _db.SaveChangesAsync(ct);

        // حذف کاربر
        var result = await _userMgr.DeleteAsync(user);

        if (!result.Succeeded)
            return Result.Failure(string.Join("، ",
                result.Errors.Select(e => e.Description)));

        return Result.Success();
    }
}