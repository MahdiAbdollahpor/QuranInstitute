using MediatR;
using QuranInstitute.Application.Common.Models;

namespace QuranInstitute.Application.Features.Classes.Queries.GetClassById;

public class GetClassByIdQuery : IRequest<Result<ClassDetailDto>>
{
    public Guid Id { get; set; }
    public GetClassByIdQuery(Guid id) => Id = id;
}