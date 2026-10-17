using MediatR;
using QuranInstitute.Application.Common.Models;

namespace QuranInstitute.Application.Features.UserPanel.Queries.GetMyDashboard;

public class GetMyDashboardQuery : IRequest<Result<MyDashboardDto>>
{
}