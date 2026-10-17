using MediatR;
using QuranInstitute.Application.Common.Models;

namespace QuranInstitute.Application.Features.UserPanel.Queries.GetMyNotifications;

public class GetMyNotificationsQuery : IRequest<Result<List<MyNotificationDto>>>
{
    public bool UnreadOnly { get; set; }
}