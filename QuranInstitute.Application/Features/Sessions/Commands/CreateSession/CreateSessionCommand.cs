using MediatR;
using QuranInstitute.Application.Common.Models;

namespace QuranInstitute.Application.Features.Sessions.Commands.CreateSession;

public class CreateSessionCommand : IRequest<Result<Guid>>
{
    public Guid ClassId { get; set; }
    public int SessionNumber { get; set; }
    public DateTime Date { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsOnline { get; set; }
    public string? OnlineLink { get; set; }
    public string? RecordingUrl { get; set; }
}