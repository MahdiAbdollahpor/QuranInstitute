using MediatR;
using QuranInstitute.Application.Common.Models;

namespace QuranInstitute.Application.Features.Notifications.Queries.GetAnnouncementById;

public class GetAnnouncementByIdQuery : IRequest<Result<AnnouncementDetailDto>>
{
    public Guid Id { get; set; }
    public GetAnnouncementByIdQuery(Guid id) => Id = id;
}

public class AnnouncementDetailDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public Guid? TargetClassId { get; set; }
    public string? TargetClassName { get; set; }
    public Guid? TargetStudentId { get; set; }
    public string? TargetStudentName { get; set; }
    public bool SentSms { get; set; }
    public bool SentWhatsApp { get; set; }
    public bool SentInApp { get; set; }
    public DateTime? SentAt { get; set; }
    public DateTime CreatedAt { get; set; }
}