using MediatR;
using QuranInstitute.Application.Common.Models;

namespace QuranInstitute.Application.Features.UserPanel.Queries.GetMyProfile;

public class GetMyProfileQuery : IRequest<Result<MyProfileDto>>
{
}