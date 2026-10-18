using MediatR;
using QuranInstitute.Application.Common.Models;

namespace QuranInstitute.Application.Features.Sessions.Queries.GetSessionsByClass;

public class GetSessionsByClassQuery : IRequest<Result<SessionsListResult>>
{
    public Guid ClassId { get; set; }
    public GetSessionsByClassQuery(Guid classId) => ClassId = classId;
}

public class SessionsListResult
{
    public Guid ClassId { get; set; }
    public string ClassName { get; set; } = string.Empty;
    public string TeacherName { get; set; } = string.Empty;
    public List<SessionListItemDto> Sessions { get; set; } = new();
}