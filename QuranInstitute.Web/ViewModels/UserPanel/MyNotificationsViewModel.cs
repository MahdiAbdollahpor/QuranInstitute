using QuranInstitute.Application.Features.UserPanel.Queries.GetMyNotifications;

namespace QuranInstitute.Web.ViewModels.UserPanel;

public class MyNotificationsViewModel
{
    public List<MyNotificationDto> Notifications { get; set; } = new();
}