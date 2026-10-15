using MediatR;

namespace QuranInstitute.Application.Features.Students.Queries.GetClassesForSelect;

public class GetClassesForSelectQuery : IRequest<List<ClassSelectItemDto>>
{
}