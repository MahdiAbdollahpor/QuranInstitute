using MediatR;
using Microsoft.EntityFrameworkCore;
using QuranInstitute.Application.Common.Interfaces;
using QuranInstitute.Application.Common.Models;
using QuranInstitute.Domain.Enums;

namespace QuranInstitute.Application.Features.Notifications.Commands.DeleteAnnouncement;

public class DeleteAnnouncementCommandHandler
    : IRequestHandler<DeleteAnnouncementCommand, Result>
{
    private readonly IApplicationDbContext _db;

    public DeleteAnnouncementCommandHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result> Handle(
        DeleteAnnouncementCommand request, CancellationToken ct)
    {
        var announcement = await _db.Announcements
            .FirstOrDefaultAsync(a => a.Id == request.Id, ct);

        if (announcement is null)
            return Result.Failure("اطلاعیه یافت نشد.");

        // حذف Notificationهای مرتبط
        var notifications = await _db.Notifications
            .Where(n => n.Type == NotificationType.Announcement
                     && n.Title == announcement.Title
                     && n.Body == announcement.Body)
            .ToListAsync(ct);

        foreach (var n in notifications)
            _db.Notifications.Remove(n);

        _db.Announcements.Remove(announcement);
        await _db.SaveChangesAsync(ct);

        return Result.Success();
    }
}