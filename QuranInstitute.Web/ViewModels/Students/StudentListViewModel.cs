using QuranInstitute.Application.Common.Models;
using QuranInstitute.Application.Features.Students.Queries.GetStudentsList;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace QuranInstitute.Web.ViewModels.Students;

public class StudentListViewModel
{
    public PagedList<StudentListItemDto> Students { get; set; } = new();
    public string? Search { get; set; }
    public Guid? ClassId { get; set; }
    public List<SelectListItem> Classes { get; set; } = new();
}