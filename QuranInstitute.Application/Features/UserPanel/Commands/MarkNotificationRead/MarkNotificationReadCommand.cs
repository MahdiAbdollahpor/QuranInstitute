using MediatR;
using QuranInstitute.Application.Common.Models;

namespace QuranInstitute.Application.Features.UserPanel.Commands.MarkNotificationRead;

public class MarkNotificationReadCommand : IRequest<Result>
{
    public Guid Id { get; set; }
    public MarkNotificationReadCommand(Guid id) => Id = id;
}

public class MarkAllNotificationsReadCommand : IRequest<Result>
{
}