using MediatR;
using QuranInstitute.Application.Common.Models;

namespace QuranInstitute.Application.Features.Students.Commands.CreateStudent;

public class CreateStudentCommand : IRequest<Result<Guid>>
{
    public string Name { get; set; } = string.Empty;
    public string ParentName { get; set; } = string.Empty;
    public string ParentMobile { get; set; } = string.Empty;
    public Guid? ClassId { get; set; }
    public decimal MonthlyFee { get; set; }
    public DateTime StartDate { get; set; }
    public string? Note { get; set; }
}