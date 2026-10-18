using MediatR;
using QuranInstitute.Application.Common.Models;

namespace QuranInstitute.Application.Features.Reports.Queries.GetDashboardStats;

public class GetDashboardStatsQuery : IRequest<Result<DashboardStatsDto>>
{
}