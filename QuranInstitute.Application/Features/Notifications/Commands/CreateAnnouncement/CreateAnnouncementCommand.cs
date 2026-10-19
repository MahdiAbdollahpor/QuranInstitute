using MediatR;
using QuranInstitute.Application.Common.Models;

namespace QuranInstitute.Application.Features.Notifications.Commands.CreateAnnouncement;

public class CreateAnnouncementCommand : IRequest<Result<Guid>>
{
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public Guid? TargetClassId { get; set; }
    public Guid? TargetStudentId { get; set; }
    public bool SendSms { get; set; }
    public bool SendWhatsApp { get; set; }
    public bool SendInApp { get; set; } = true;
}