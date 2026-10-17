using MediatR;
using Microsoft.EntityFrameworkCore;
using QuranInstitute.Application.Common.Interfaces;
using QuranInstitute.Application.Common.Models;

namespace QuranInstitute.Application.Features.UserPanel.Queries.GetMyNotifications;

public class GetMyNotificationsQueryHandler
    : IRequestHandler<GetMyNotificationsQuery, Result<List<MyNotificationDto>>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _current;

    public GetMyNotificationsQueryHandler(
        IApplicationDbContext db,
        ICurrentUserService current)
    {
        _db = db;
        _current = current;
    }

    public async Task<Result<List<MyNotificationDto>>> Handle(
        GetMyNotificationsQuery request, CancellationToken ct)
    {
        var userId = _current.UserId;
        if (!userId.HasValue)
            return Result<List<MyNotificationDto>>.Failure("کاربر شناسایی نشد.");

        var studentIds = await _db.Students
            .Where(s => s.UserId == userId.Value && s.Active)
            .Select(s => s.Id)
            .ToListAsync(ct);

        var q = _db.Notifications
            .AsNoTracking()
            .Where(n => n.UserId == userId.Value
                     || (n.StudentId.HasValue && studentIds.Contains(n.StudentId.Value)));

        if (request.UnreadOnly)
            q = q.Where(n => !n.IsRead);

        var items = await q
            .OrderByDescending(n => n.CreatedAt)
            .Take(100)
            .Select(n => new MyNotificationDto
            {
                Id = n.Id,
                Type = n.Type,
                Title = n.Title,
                Body = n.Body,
                IsRead = n.IsRead,
                CreatedAt = n.CreatedAt
            })
            .ToListAsync(ct);

        return Result<List<MyNotificationDto>>.Success(items);
    }
}