using MediatR;
using Microsoft.EntityFrameworkCore;
using QuranInstitute.Application.Common.Interfaces;
using QuranInstitute.Application.Common.Models;
using QuranInstitute.Domain.Enums;

namespace QuranInstitute.Application.Features.Notifications.Queries.GetSmsStats;

public class GetSmsStatsQueryHandler
    : IRequestHandler<GetSmsStatsQuery, Result<SmsStatsDto>>
{
    private readonly IApplicationDbContext _db;

    public GetSmsStatsQueryHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result<SmsStatsDto>> Handle(
        GetSmsStatsQuery request, CancellationToken ct)
    {
        var today = DateTime.Today;
        var monthStart = new DateTime(today.Year, today.Month, 1);
        var monthEnd = monthStart.AddMonths(1);

        var stats = new SmsStatsDto
        {
            TotalSent = await _db.SmsLogs
                .CountAsync(x => x.Status == SmsStatus.Sent, ct),

            SentThisMonth = await _db.SmsLogs
                .CountAsync(x => x.Status == SmsStatus.Sent
                              && x.CreatedAt >= monthStart
                              && x.CreatedAt < monthEnd, ct),

            FailedThisMonth = await _db.SmsLogs
                .CountAsync(x => x.Status == SmsStatus.Failed
                              && x.CreatedAt >= monthStart
                              && x.CreatedAt < monthEnd, ct),

            AbsentCount = await _db.SmsLogs
                .CountAsync(x => x.Type == SmsType.Absent, ct),

            DebtCount = await _db.SmsLogs
                .CountAsync(x => x.Type == SmsType.Debt, ct),

            AnnouncementCount = await _db.SmsLogs
                .CountAsync(x => x.Type == SmsType.Announcement, ct),

            ManualCount = await _db.SmsLogs
                .CountAsync(x => x.Type == SmsType.Manual, ct)
        };

        return Result<SmsStatsDto>.Success(stats);
    }
}