using Microsoft.AspNetCore.Mvc.Rendering;
using QuranInstitute.Application.Common.Models;
using QuranInstitute.Application.Features.Admin.Users.Queries.GetUsersList;

namespace QuranInstitute.Web.ViewModels.Admin;

public class UserListViewModel
{
    public PagedList<UserListItemDto> Users { get; set; } = new();
    public string? Search { get; set; }
    public string? RoleFilter { get; set; }
    public bool? ActiveOnly { get; set; }

    public List<SelectListItem> RoleOptions { get; set; } = new();
}