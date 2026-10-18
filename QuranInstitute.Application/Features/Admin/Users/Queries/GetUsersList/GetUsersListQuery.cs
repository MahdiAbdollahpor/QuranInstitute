using MediatR;
using QuranInstitute.Application.Common.Models;

namespace QuranInstitute.Application.Features.Admin.Users.Queries.GetUsersList;

public class GetUsersListQuery : IRequest<Result<PagedList<UserListItemDto>>>
{
    public string? Search { get; set; }
    public string? RoleFilter { get; set; }       // Admin | Teacher | Parent
    public bool? ActiveOnly { get; set; }         // null = همه
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 30;
}