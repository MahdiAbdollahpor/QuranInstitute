using MediatR;
using QuranInstitute.Application.Common.Models;
using QuranInstitute.Domain.Enums;

namespace QuranInstitute.Application.Features.Classes.Commands.CreateClass;

public class CreateClassCommand : IRequest<Result<Guid>>
{
    public string Name { get; set; } = string.Empty;
    public string TeacherName { get; set; } = string.Empty;
    public string? ClassTime { get; set; }
    public int[] Days { get; set; } = Array.Empty<int>();
    public ClassType Type { get; set; } = ClassType.InPerson;
    public string? DefaultOnlineLink { get; set; }
    public int Capacity { get; set; } = 30;
}