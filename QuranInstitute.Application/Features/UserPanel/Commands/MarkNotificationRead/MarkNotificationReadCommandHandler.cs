using MediatR;
using Microsoft.EntityFrameworkCore;
using QuranInstitute.Application.Common.Interfaces;
using QuranInstitute.Application.Common.Models;

namespace QuranInstitute.Application.Features.UserPanel.Commands.MarkNotificationRead;

public class MarkNotificationReadCommandHandler
    : IRequestHandler<MarkNotificationReadCommand, Result>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _current;

    public MarkNotificationReadCommandHandler(
        IApplicationDbContext db,
        ICurrentUserService current)
    {
        _db = db;
        _current = current;
    }

    public async Task<Result> Handle(
        MarkNotificationReadCommand request, CancellationToken ct)
    {
        var userId = _current.UserId;
        if (!userId.HasValue)
            return Result.Failure("کاربر شناسایی نشد.");

        var n = await _db.Notifications
            .FirstOrDefaultAsync(x => x.Id == request.Id
                                   && x.UserId == userId.Value, ct);

        if (n is null)
            return Result.Failure("اعلان یافت نشد.");

        n.IsRead = true;
        n.ReadAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
        return Result.Success();
    }
}

public class MarkAllNotificationsReadCommandHandler
    : IRequestHandler<MarkAllNotificationsReadCommand, Result>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _current;

    public MarkAllNotificationsReadCommandHandler(
        IApplicationDbContext db,
        ICurrentUserService current)
    {
        _db = db;
        _current = current;
    }

    public async Task<Result> Handle(
        MarkAllNotificationsReadCommand request, CancellationToken ct)
    {
        var userId = _current.UserId;
        if (!userId.HasValue)
            return Result.Failure("کاربر شناسایی نشد.");

        var items = await _db.Notifications
            .Where(n => n.UserId == userId.Value && !n.IsRead)
            .ToListAsync(ct);

        foreach (var n in items)
        {
            n.IsRead = true;
            n.ReadAt = DateTime.UtcNow;
        }

        await _db.SaveChangesAsync(ct);
        return Result.Success();
    }
}