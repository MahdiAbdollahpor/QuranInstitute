using MediatR;
using QuranInstitute.Application.Common.Models;

namespace QuranInstitute.Application.Features.Admin.Users.Queries.GetUserDetails;

public class GetUserDetailsQuery : IRequest<Result<UserDetailsDto>>
{
    public Guid UserId { get; set; }
    public GetUserDetailsQuery(Guid userId) => UserId = userId;
}