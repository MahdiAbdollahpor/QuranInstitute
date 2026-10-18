using QuranInstitute.Application.Features.Admin.Users.Queries.GetUserDetails;

namespace QuranInstitute.Web.ViewModels.Admin;

public class UserDetailsViewModel
{
    public UserDetailsDto User { get; set; } = new();
    public bool IsCurrentUser { get; set; }
}