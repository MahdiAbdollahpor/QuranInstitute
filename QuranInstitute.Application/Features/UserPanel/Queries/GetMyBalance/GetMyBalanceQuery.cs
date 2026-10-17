using MediatR;
using QuranInstitute.Application.Common.Models;

namespace QuranInstitute.Application.Features.UserPanel.Queries.GetMyBalance;

public class GetMyBalanceQuery : IRequest<Result<MyBalanceDto>>
{
    public Guid StudentId { get; set; }
    public GetMyBalanceQuery(Guid studentId) => StudentId = studentId;
}