using MediatR;
using QuranInstitute.Application.Common.Models;

namespace QuranInstitute.Application.Features.Notifications.Queries.GetSmsStats;

public class GetSmsStatsQuery : IRequest<Result<SmsStatsDto>>
{
}

public class SmsStatsDto
{
    public int TotalSent { get; set; }
    public int SentThisMonth { get; set; }
    public int FailedThisMonth { get; set; }
    public int AbsentCount { get; set; }
    public int DebtCount { get; set; }
    public int AnnouncementCount { get; set; }
    public int ManualCount { get; set; }
}