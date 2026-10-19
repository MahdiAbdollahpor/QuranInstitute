using MediatR;
using QuranInstitute.Application.Common.Models;

namespace QuranInstitute.Application.Features.Notifications.Queries.PreviewDebtSms;

public class PreviewDebtSmsQuery : IRequest<Result<DebtSmsPreviewResult>>
{
    public Guid? ClassId { get; set; }
}

public class DebtSmsPreviewResult
{
    public string Template { get; set; } = string.Empty;
    public List<DebtRecipientPreview> Recipients { get; set; } = new();
}

public class DebtRecipientPreview
{
    public Guid StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string ParentName { get; set; } = string.Empty;
    public string Mobile { get; set; } = string.Empty;
    public decimal Balance { get; set; }
    public string RenderedMessage { get; set; } = string.Empty;
}