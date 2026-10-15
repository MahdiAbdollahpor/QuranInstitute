using Microsoft.AspNetCore.Mvc.Rendering;
using QuranInstitute.Application.Common.Models;
using QuranInstitute.Application.Features.Classes.Queries.GetClassesList;

namespace QuranInstitute.Web.ViewModels.Classes;

public class ClassListViewModel
{
    public PagedList<ClassListItemDto> Classes { get; set; } = new();
    public string? Search { get; set; }
}