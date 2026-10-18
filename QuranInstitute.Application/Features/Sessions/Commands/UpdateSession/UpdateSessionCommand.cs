using MediatR;
using QuranInstitute.Application.Common.Models;

namespace QuranInstitute.Application.Features.Sessions.Commands.UpdateSession;

public class UpdateSessionCommand : IRequest<Result>
{
    public Guid Id { get; set; }
    public int SessionNumber { get; set; }
    public DateTime Date { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsOnline { get; set; }
    public string? OnlineLink { get; set; }
    public string? RecordingUrl { get; set; }
}