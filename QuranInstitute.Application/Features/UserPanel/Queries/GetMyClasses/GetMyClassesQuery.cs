using MediatR;
using QuranInstitute.Application.Common.Models;

namespace QuranInstitute.Application.Features.UserPanel.Queries.GetMyClasses;

public class GetMyClassesQuery : IRequest<Result<List<MyClassDto>>>
{
    public Guid? StudentId { get; set; }
}