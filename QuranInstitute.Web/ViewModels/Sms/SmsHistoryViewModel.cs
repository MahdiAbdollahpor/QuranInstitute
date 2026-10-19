using Microsoft.AspNetCore.Mvc.Rendering;
using QuranInstitute.Application.Common.Models;
using QuranInstitute.Application.Features.Notifications.Queries.GetSmsHistory;
using QuranInstitute.Application.Features.Notifications.Queries.GetSmsStats;
using QuranInstitute.Domain.Enums;

namespace QuranInstitute.Web.ViewModels.Sms;

public class SmsHistoryViewModel
{
    public PagedList<SmsHistoryItemDto> Logs { get; set; } = new();
    public SmsStatsDto Stats { get; set; } = new();

    public string? Search { get; set; }
    public SmsType? Type { get; set; }
    public SmsStatus? Status { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }

    public List<SelectListItem> TypeOptions { get; set; } = new();
    public List<SelectListItem> StatusOptions { get; set; } = new();
}