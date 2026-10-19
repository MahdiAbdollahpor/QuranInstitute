using MediatR;
using Microsoft.EntityFrameworkCore;
using QuranInstitute.Application.Common.Interfaces;
using QuranInstitute.Application.Common.Models;
using QuranInstitute.Domain.Entities.Communication;
using QuranInstitute.Domain.Entities.Education;
using QuranInstitute.Domain.Enums;

namespace QuranInstitute.Application.Features.Notifications.Commands.CreateAnnouncement;

public class CreateAnnouncementCommandHandler
    : IRequestHandler<CreateAnnouncementCommand, Result<Guid>>
{
    private readonly IApplicationDbContext _db;
    private readonly ISmsService _sms;
    private readonly ICurrentUserService _current;

    public CreateAnnouncementCommandHandler(
        IApplicationDbContext db,
        ISmsService sms,
        ICurrentUserService current)
    {
        _db = db;
        _sms = sms;
        _current = current;
    }

    public async Task<Result<Guid>> Handle(
        CreateAnnouncementCommand request, CancellationToken ct)
    {
        var userId = _current.UserId;
        if (!userId.HasValue)
            return Result<Guid>.Failure("کاربر شناسایی نشد.");

        // ─── بررسی کلاس/شاگرد ───
        if (request.TargetClassId.HasValue)
        {
            var clsExists = await _db.Classes
                .AnyAsync(c => c.Id == request.TargetClassId.Value, ct);
            if (!clsExists)
                return Result<Guid>.Failure("کلاس یافت نشد.");
        }

        if (request.TargetStudentId.HasValue)
        {
            var stuExists = await _db.Students
                .AnyAsync(s => s.Id == request.TargetStudentId.Value, ct);
            if (!stuExists)
                return Result<Guid>.Failure("شاگرد یافت نشد.");
        }

        // ─── ساخت اطلاعیه ───
        var announcement = new Announcement
        {
            Title = request.Title.Trim(),
            Body = request.Body.Trim(),
            TargetClassId = request.TargetClassId,
            TargetStudentId = request.TargetStudentId,
            SendSms = request.SendSms,
            SendWhatsApp = request.SendWhatsApp,
            SendInApp = request.SendInApp,
            CreatedByUserId = userId.Value
        };

        _db.Announcements.Add(announcement);

        // ─── ساخت Notification برای گیرندگان (In-App) ───
        if (request.SendInApp)
        {
            var recipients = await GetRecipientsAsync(request, ct);

            foreach (var r in recipients)
            {
                _db.Notifications.Add(new Notification
                {
                    StudentId = r.StudentId,
                    UserId = r.UserId,
                    Type = NotificationType.Announcement,
                    Title = request.Title.Trim(),
                    Body = request.Body.Trim(),
                    IsRead = false
                });
            }
        }

        announcement.SentAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);

        // ─── ارسال SMS (بعد از ذخیره، چون ممکن است کند باشد) ───
        if (request.SendSms)
        {
            var recipients = await GetRecipientsAsync(request, ct);

            var smsRequests = recipients
                .Where(r => !string.IsNullOrWhiteSpace(r.Mobile))
                .Select(r => new SmsSendRequest
                {
                    Mobile = r.Mobile,
                    Message = $"{request.Title}\n{request.Body}",
                    Type = SmsType.Announcement,
                    StudentId = r.StudentId
                   // RecipientName = r.ParentName  // اگر داری
                })
                .ToList();

            if (smsRequests.Any())
                await _sms.SendBulkAsync(smsRequests, ct);
        }

        return Result<Guid>.Success(announcement.Id);
    }

    // ─── دریافت گیرندگان ───
    private async Task<List<RecipientInfo>> GetRecipientsAsync(
        CreateAnnouncementCommand request, CancellationToken ct)
    {
        IQueryable<Student> query = _db.Students
            .Where(s => s.Active);

        if (request.TargetStudentId.HasValue)
        {
            query = query.Where(s => s.Id == request.TargetStudentId.Value);
        }
        else if (request.TargetClassId.HasValue)
        {
            query = query.Where(s => s.Enrollments.Any(e =>
                e.ClassId == request.TargetClassId.Value &&
                e.Status == EnrollmentStatus.Approved));
        }

        return await query
            .Select(s => new RecipientInfo
            {
                StudentId = s.Id,
                UserId = s.UserId,
                Mobile = s.ParentMobile
            })
            .ToListAsync(ct);
    }

    private async Task<List<string>> GetRecipientMobilesAsync(
        CreateAnnouncementCommand request, CancellationToken ct)
    {
        var recipients = await GetRecipientsAsync(request, ct);
        return recipients
            .Select(r => r.Mobile)
            .Where(m => !string.IsNullOrWhiteSpace(m))
            .Distinct()
            .ToList();
    }

    private class RecipientInfo
    {
        public Guid StudentId { get; set; }
        public Guid? UserId { get; set; }
        public string Mobile { get; set; } = string.Empty;
    }
}