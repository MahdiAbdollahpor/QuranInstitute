using MediatR;
using QuranInstitute.Application.Common.Models;

namespace QuranInstitute.Application.Features.Tuition.Queries.GetStudentBalance;

public class GetStudentBalanceQuery : IRequest<Result<StudentBalanceDto>>
{
    public Guid StudentId { get; set; }
    public GetStudentBalanceQuery(Guid studentId) => StudentId = studentId;
}