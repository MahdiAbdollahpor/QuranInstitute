using MediatR;
using QuranInstitute.Application.Common.Models;
using QuranInstitute.Application.Features.Resources.Queries.GetResourcesByClass;

namespace QuranInstitute.Application.Features.Resources.Queries.GetResourcesBySession;

public class GetResourcesBySessionQuery : IRequest<Result<List<ResourceListItemDto>>>
{
    public Guid SessionId { get; set; }
    public GetResourcesBySessionQuery(Guid sessionId) => SessionId = sessionId;
}