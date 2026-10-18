namespace QuranInstitute.Application.Features.Sessions.Queries.GetSessionById;

public class SessionDetailDto
{
    public Guid Id { get; set; }
    public Guid ClassId { get; set; }
    public string ClassName { get; set; } = string.Empty;
    public int SessionNumber { get; set; }
    public DateTime Date { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsOnline { get; set; }
    public string? OnlineLink { get; set; }
    public string? RecordingUrl { get; set; }
}