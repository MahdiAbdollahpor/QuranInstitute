using MediatR;
using QuranInstitute.Application.Common.Models;

namespace QuranInstitute.Application.Features.Resources.Queries.GetResourcesByClass;

public class GetResourcesByClassQuery : IRequest<Result<List<ResourceListItemDto>>>
{
    public Guid ClassId { get; set; }
    public Guid? SessionId { get; set; }

    public GetResourcesByClassQuery(Guid classId, Guid? sessionId = null)
    {
        ClassId = classId;
        SessionId = sessionId;
    }
}