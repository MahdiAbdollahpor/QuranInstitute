using MediatR;
using QuranInstitute.Application.Common.Models;

namespace QuranInstitute.Application.Features.Sessions.Queries.GetSessionById;

public class GetSessionByIdQuery : IRequest<Result<SessionDetailDto>>
{
    public Guid Id { get; set; }
    public GetSessionByIdQuery(Guid id) => Id = id;
}