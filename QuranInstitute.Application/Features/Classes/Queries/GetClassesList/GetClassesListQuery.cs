using MediatR;
using QuranInstitute.Application.Common.Models;

namespace QuranInstitute.Application.Features.Classes.Queries.GetClassesList;

public class GetClassesListQuery : IRequest<PagedList<ClassListItemDto>>
{
    public string? Search { get; set; }
    public bool? ActiveOnly { get; set; } = true;
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}