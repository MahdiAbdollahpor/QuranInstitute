using MediatR;
using QuranInstitute.Application.Common.Models;

namespace QuranInstitute.Application.Features.Tuition.Queries.GetMonthlyIncome;

public class GetMonthlyIncomeQuery : IRequest<Result<MonthlyIncomeDto>>
{
    /// <summary>اگر null باشد، از ماه جاری میلادی استفاده می‌شود</summary>
    public int? Year { get; set; }
    public int? Month { get; set; }
}