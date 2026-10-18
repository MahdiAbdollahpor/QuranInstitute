using QuranInstitute.Application.Features.Sessions.Queries.GetSessionsByClass;

namespace QuranInstitute.Web.ViewModels.Sessions;

public class SessionsListViewModel
{
    public SessionsListResult Data { get; set; } = new();
}