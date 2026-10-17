using MediatR;
using QuranInstitute.Application.Common.Models;

namespace QuranInstitute.Application.Features.UserPanel.Queries.GetAvailableClasses;

public class GetAvailableClassesQuery : IRequest<Result<List<AvailableClassDto>>>
{
    public Guid? StudentId { get; set; }
}