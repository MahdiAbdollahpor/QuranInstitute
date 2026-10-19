using QuranInstitute.Application.Features.Notifications.Queries.PreviewAbsentSms;
using QuranInstitute.Application.Features.Notifications.Queries.PreviewDebtSms;

namespace QuranInstitute.Web.ViewModels.Sms;

public class AbsentSmsPreviewViewModel
{
    public SmsPreviewResult Data { get; set; } = new();
    public DateTime SelectedDate { get; set; }
    public Guid? ClassId { get; set; }
    public List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> Classes { get; set; } = new();
}

public class DebtSmsPreviewViewModel
{
    public DebtSmsPreviewResult Data { get; set; } = new();
    public Guid? ClassId { get; set; }
    public List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> Classes { get; set; } = new();
}