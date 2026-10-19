using MediatR;
using Microsoft.EntityFrameworkCore;
using QuranInstitute.Application.Common.Interfaces;
using QuranInstitute.Application.Common.Models;

namespace QuranInstitute.Application.Features.Notifications.Queries.GetAnnouncementById;

public class GetAnnouncementByIdQueryHandler
    : IRequestHandler<GetAnnouncementByIdQuery, Result<AnnouncementDetailDto>>
{
    private readonly IApplicationDbContext _db;

    public GetAnnouncementByIdQueryHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result<AnnouncementDetailDto>> Handle(
        GetAnnouncementByIdQuery request, CancellationToken ct)
    {
        var raw = await _db.Announcements
            .AsNoTracking()
            .Where(a => a.Id == request.Id)
            .Select(a => new
            {
                a.Id,
                a.Title,
                a.Body,
                a.TargetClassId,
                TargetClassName = a.TargetClassId == null ? null
                    : _db.Classes.Where(c => c.Id == a.TargetClassId).Select(c => c.Name).FirstOrDefault(),
                a.TargetStudentId,
                TargetStudentName = a.TargetStudentId == null ? null
                    : _db.Students.Where(s => s.Id == a.TargetStudentId).Select(s => s.Name).FirstOrDefault(),
                a.SendSms,
                a.SendWhatsApp,
                a.SendInApp,
                a.SentAt,
                a.CreatedAt
            })
            .FirstOrDefaultAsync(ct);

        if (raw is null)
            return Result<AnnouncementDetailDto>.Failure("اطلاعیه یافت نشد.");

        return Result<AnnouncementDetailDto>.Success(new AnnouncementDetailDto
        {
            Id = raw.Id,
            Title = raw.Title,
            Body = raw.Body,
            TargetClassId = raw.TargetClassId,
            TargetClassName = raw.TargetClassName,
            TargetStudentId = raw.TargetStudentId,
            TargetStudentName = raw.TargetStudentName,
            SentSms = raw.SendSms,
            SentWhatsApp = raw.SendWhatsApp,
            SentInApp = raw.SendInApp,
            SentAt = raw.SentAt,
            CreatedAt = raw.CreatedAt
        });
    }
}