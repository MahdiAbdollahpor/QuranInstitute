using MediatR;
using QuranInstitute.Application.Common.Models;

namespace QuranInstitute.Application.Features.Attendances.Commands.MarkAllPresent;

public class MarkAllPresentCommand : IRequest<Result>
{
    public Guid ClassId { get; set; }
    public DateTime Date { get; set; }

    public MarkAllPresentCommand(Guid classId, DateTime date)
    {
        ClassId = classId;
        Date = date;
    }
}