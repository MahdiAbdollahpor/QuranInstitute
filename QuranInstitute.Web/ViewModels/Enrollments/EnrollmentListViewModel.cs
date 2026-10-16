using Microsoft.AspNetCore.Mvc.Rendering;
using QuranInstitute.Application.Common.Models;
using QuranInstitute.Application.Features.Enrollments.Queries.GetEnrollmentsList;
using QuranInstitute.Domain.Enums;

namespace QuranInstitute.Web.ViewModels.Enrollments;

public class EnrollmentListViewModel
{
    public PagedList<EnrollmentListItemDto> Enrollments { get; set; } = new();
    public string? Search { get; set; }
    public Guid? ClassId { get; set; }
    public EnrollmentStatus? Status { get; set; }

    public List<SelectListItem> Classes { get; set; } = new();
    public List<SelectListItem> StatusOptions { get; set; } = new();
}