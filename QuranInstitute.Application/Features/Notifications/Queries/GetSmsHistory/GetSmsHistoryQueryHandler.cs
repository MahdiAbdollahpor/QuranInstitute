using MediatR;
using Microsoft.EntityFrameworkCore;
using QuranInstitute.Application.Common.Interfaces;
using QuranInstitute.Application.Common.Models;

namespace QuranInstitute.Application.Features.Notifications.Queries.GetSmsHistory;

public class GetSmsHistoryQueryHandler
    : IRequestHandler<GetSmsHistoryQuery, PagedList<SmsHistoryItemDto>>
{
    private readonly IApplicationDbContext _db;

    public GetSmsHistoryQueryHandler(IApplicationDbContext db) => _db = db;

    public async Task<PagedList<SmsHistoryItemDto>> Handle(
        GetSmsHistoryQuery request, CancellationToken ct)
    {
        var q = _db.SmsLogs.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var s = request.Search.Trim();
            q = q.Where(x =>
                x.Mobile.Contains(s) ||
                x.Message.Contains(s) ||
                (x.RecipientName != null && x.RecipientName.Contains(s)));
        }

        if (request.Type.HasValue)
            q = q.Where(x => x.Type == request.Type.Value);

        if (request.Status.HasValue)
            q = q.Where(x => x.Status == request.Status.Value);

        if (request.FromDate.HasValue)
            q = q.Where(x => x.CreatedAt >= request.FromDate.Value.Date);

        if (request.ToDate.HasValue)
            q = q.Where(x => x.CreatedAt <= request.ToDate.Value.Date.AddDays(1));

        if (request.StudentId.HasValue)
            q = q.Where(x => x.StudentId == request.StudentId.Value);

        var total = await q.CountAsync(ct);

        var items = await q
            .OrderByDescending(x => x.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(x => new SmsHistoryItemDto
            {
                Id = x.Id,
                StudentId = x.StudentId,
                StudentName = x.Student != null ? x.Student.Name : null,
                Mobile = x.Mobile,
                RecipientName = x.RecipientName,
                Message = x.Message,
                Type = x.Type,
                Status = x.Status,
                ErrorMessage = x.ErrorMessage,
                ProviderMessageId = x.ProviderMessageId,
                Provider = x.Provider,
                SentAt = x.SentAt,
                CreatedAt = x.CreatedAt
            })
            .ToListAsync(ct);

        return new PagedList<SmsHistoryItemDto>
        {
            Items = items,
            TotalCount = total,
            Page = request.Page,
            PageSize = request.PageSize
        };
    }
}