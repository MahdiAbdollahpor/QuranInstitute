using MediatR;
using QuranInstitute.Application.Common.Models;

namespace QuranInstitute.Application.Features.Tuition.Queries.GetDebtorsList;

public class GetDebtorsListQuery : IRequest<PagedList<DebtorItemDto>>
{
    public string? Search { get; set; }
    public Guid? ClassId { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 50;
}