using MediatR;
using QuranInstitute.Application.Common.Models;
using QuranInstitute.Domain.Enums;

namespace QuranInstitute.Application.Features.Classes.Commands.UpdateClass;

public class UpdateClassCommand : IRequest<Result>
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string TeacherName { get; set; } = string.Empty;
    public string? ClassTime { get; set; }
    public int[] Days { get; set; } = Array.Empty<int>();
    public ClassType Type { get; set; }
    public string? DefaultOnlineLink { get; set; }
    public int Capacity { get; set; } = 30;
}