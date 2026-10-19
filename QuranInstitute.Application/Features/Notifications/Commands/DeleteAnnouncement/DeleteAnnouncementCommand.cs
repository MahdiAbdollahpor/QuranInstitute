using MediatR;
using QuranInstitute.Application.Common.Models;

namespace QuranInstitute.Application.Features.Notifications.Commands.DeleteAnnouncement;

public class DeleteAnnouncementCommand : IRequest<Result>
{
    public Guid Id { get; set; }
    public DeleteAnnouncementCommand(Guid id) => Id = id;
}