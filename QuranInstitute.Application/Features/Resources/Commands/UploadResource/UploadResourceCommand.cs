using MediatR;
using QuranInstitute.Application.Common.Models;
using QuranInstitute.Domain.Enums;

namespace QuranInstitute.Application.Features.Resources.Commands.UploadResource;

public class UploadResourceCommand : IRequest<Result<Guid>>
{
    public Guid ClassId { get; set; }
    public Guid? SessionId { get; set; }
    public ResourceType Type { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool EnrolledOnly { get; set; } = true;

    // ★ برای فایل واقعی
    public Stream? FileStream { get; set; }
    public string? FileName { get; set; }
    public string? ContentType { get; set; }
    public long? FileSize { get; set; }

    // ★ برای لینک خارجی (نوع Link)
    public string? ExternalUrl { get; set; }
}