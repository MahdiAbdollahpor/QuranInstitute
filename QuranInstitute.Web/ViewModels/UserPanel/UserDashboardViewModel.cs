using QuranInstitute.Application.Features.UserPanel.Queries.GetMyDashboard;

namespace QuranInstitute.Web.ViewModels.UserPanel;

public class UserDashboardViewModel
{
    public MyDashboardDto Data { get; set; } = new();
}