using MediatR;
using Microsoft.EntityFrameworkCore;
using QuranInstitute.Application.Common.Interfaces;
using QuranInstitute.Application.Common.Models;

namespace QuranInstitute.Application.Features.Notifications.Queries.GetAnnouncementsList;

public class GetAnnouncementsListQueryHandler
    : IRequestHandler<GetAnnouncementsListQuery, PagedList<AnnouncementListItemDto>>
{
    private readonly IApplicationDbContext _db;

    public GetAnnouncementsListQueryHandler(IApplicationDbContext db) => _db = db;

    public async Task<PagedList<AnnouncementListItemDto>> Handle(
        GetAnnouncementsListQuery request, CancellationToken ct)
    {
        var q = _db.Announcements.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var s = request.Search.Trim();
            q = q.Where(a => a.Title.Contains(s) || a.Body.Contains(s));
        }

        var total = await q.CountAsync(ct);

        var items = await q
            .OrderByDescending(a => a.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(a => new AnnouncementListItemDto
            {
                Id = a.Id,
                Title = a.Title,
                Body = a.Body,
                TargetClassId = a.TargetClassId,
                TargetStudentId = a.TargetStudentId,
                SentSms = a.SendSms,
                SentWhatsApp = a.SendWhatsApp,
                SentInApp = a.SendInApp,
                SentAt = a.SentAt,
                CreatedAt = a.CreatedAt,
                RecipientCount = 0    // در گام بعد می‌توانیم دقیق حساب کنیم
            })
            .ToListAsync(ct);

        return new PagedList<AnnouncementListItemDto>
        {
            Items = items,
            TotalCount = total,
            Page = request.Page,
            PageSize = request.PageSize
        };
    }
}