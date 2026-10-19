using MediatR;
using QuranInstitute.Application.Common.Models;
using QuranInstitute.Domain.Enums;

namespace QuranInstitute.Application.Features.Notifications.Queries.GetSmsHistory;

public class GetSmsHistoryQuery : IRequest<PagedList<SmsHistoryItemDto>>
{
    public string? Search { get; set; }
    public SmsType? Type { get; set; }
    public SmsStatus? Status { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public Guid? StudentId { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 30;
}