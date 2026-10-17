using Microsoft.AspNetCore.Mvc.Rendering;
using QuranInstitute.Application.Common.Models;
using QuranInstitute.Application.Features.Tuition.Queries.GetDebtorsList;

namespace QuranInstitute.Web.ViewModels.Tuition;

public class TuitionListViewModel
{
    public PagedList<DebtorItemDto> Debtors { get; set; } = new();
    public string? Search { get; set; }
    public Guid? ClassId { get; set; }
    public List<SelectListItem> Classes { get; set; } = new();
}