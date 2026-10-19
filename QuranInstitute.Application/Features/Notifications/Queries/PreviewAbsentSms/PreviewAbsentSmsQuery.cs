using MediatR;
using QuranInstitute.Application.Common.Models;

namespace QuranInstitute.Application.Features.Notifications.Queries.PreviewAbsentSms;

public class PreviewAbsentSmsQuery : IRequest<Result<SmsPreviewResult>>
{
    public DateTime Date { get; set; }
    public Guid? ClassId { get; set; }
}

public class SmsPreviewResult
{
    public string Template { get; set; } = string.Empty;
    public List<SmsRecipientPreview> Recipients { get; set; } = new();
}

public class SmsRecipientPreview
{
    public Guid StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string ParentName { get; set; } = string.Empty;
    public string Mobile { get; set; } = string.Empty;
    public string RenderedMessage { get; set; } = string.Empty;
}