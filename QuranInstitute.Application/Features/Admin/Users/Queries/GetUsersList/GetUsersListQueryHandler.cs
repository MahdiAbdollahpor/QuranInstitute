using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using QuranInstitute.Application.Common.Interfaces;
using QuranInstitute.Application.Common.Models;
using QuranInstitute.Domain.Entities.Users;

namespace QuranInstitute.Application.Features.Admin.Users.Queries.GetUsersList;

public class GetUsersListQueryHandler
    : IRequestHandler<GetUsersListQuery, Result<PagedList<UserListItemDto>>>
{
    private readonly IApplicationDbContext _db;
    private readonly UserManager<AppUser> _userMgr;

    public GetUsersListQueryHandler(
        IApplicationDbContext db,
        UserManager<AppUser> userMgr)
    {
        _db = db;
        _userMgr = userMgr;
    }

    public async Task<Result<PagedList<UserListItemDto>>> Handle(
        GetUsersListQuery request, CancellationToken ct)
    {
        var query = _userMgr.Users.AsNoTracking().AsQueryable();

        // ─── فیلتر جستجو ───
        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var s = request.Search.Trim();
            query = query.Where(u =>
                u.FullName.Contains(s) ||
                (u.UserName != null && u.UserName.Contains(s)) ||
                (u.PhoneNumber != null && u.PhoneNumber.Contains(s)) ||
                (u.NationalCode != null && u.NationalCode.Contains(s)));
        }

        // ─── فیلتر فعال/غیرفعال ───
        if (request.ActiveOnly.HasValue)
            query = query.Where(u => u.IsActive == request.ActiveOnly.Value);

        // ─── فیلتر نقش (ساده) ───
        if (!string.IsNullOrWhiteSpace(request.RoleFilter))
        {
            var usersInRole = await _userMgr.GetUsersInRoleAsync(request.RoleFilter);

            if (usersInRole.Any())
            {
                var allowedIds = usersInRole.Select(u => u.Id).ToList();
                query = query.Where(u => allowedIds.Contains(u.Id));
            }
            else
            {
                // هیچ کاربری این نقش را ندارد → لیست خالی
                query = query.Where(u => false);
            }
        }

        var total = await query.CountAsync(ct);

        var users = await query
            .OrderByDescending(u => u.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(ct);

        // ─── تعداد شاگردان مرتبط ───
        var userIds = users.Select(u => u.Id).ToList();

        var studentCounts = await _db.Students
            .Where(s => s.UserId.HasValue && userIds.Contains(s.UserId.Value))
            .GroupBy(s => s.UserId!.Value)
            .Select(g => new { UserId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.UserId, x => x.Count, ct);

        // ─── ساخت خروجی ───
        var items = new List<UserListItemDto>();
        foreach (var u in users)
        {
            var roles = await _userMgr.GetRolesAsync(u);

            items.Add(new UserListItemDto
            {
                Id = u.Id,
                FullName = u.FullName,
                Mobile = u.PhoneNumber ?? u.UserName ?? "",
                NationalCode = u.NationalCode,
                IsActive = u.IsActive,
                CreatedAt = u.CreatedAt,
                LastLoginAt = u.LastLoginAt,
                Roles = roles.ToList(),
                LinkedStudentsCount = studentCounts.TryGetValue(u.Id, out var c) ? c : 0
            });
        }

        return Result<PagedList<UserListItemDto>>.Success(new PagedList<UserListItemDto>
        {
            Items = items,
            TotalCount = total,
            Page = request.Page,
            PageSize = request.PageSize
        });
    }
}