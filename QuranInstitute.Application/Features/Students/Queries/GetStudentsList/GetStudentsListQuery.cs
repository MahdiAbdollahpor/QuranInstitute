using MediatR;
using QuranInstitute.Application.Common.Models;

namespace QuranInstitute.Application.Features.Students.Queries.GetStudentsList;

public class GetStudentsListQuery : IRequest<PagedList<StudentListItemDto>>
{
    public string? Search { get; set; }
    public Guid? ClassId { get; set; }
    public bool? ActiveOnly { get; set; } = true;
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}