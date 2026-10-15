using MediatR;
using QuranInstitute.Application.Common.Models;

namespace QuranInstitute.Application.Features.Students.Queries.GetStudentById;

public class GetStudentByIdQuery : IRequest<Result<StudentDetailDto>>
{
    public Guid Id { get; set; }
    public GetStudentByIdQuery(Guid id) => Id = id;
}