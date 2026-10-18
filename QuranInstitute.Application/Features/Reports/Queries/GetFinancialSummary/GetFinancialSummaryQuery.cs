using MediatR;
using QuranInstitute.Application.Common.Models;

namespace QuranInstitute.Application.Features.Reports.Queries.GetFinancialSummary;

public class GetFinancialSummaryQuery : IRequest<Result<FinancialSummaryDto>>
{
    public int JalaliYear { get; set; }
    public int JalaliMonth { get; set; }
}