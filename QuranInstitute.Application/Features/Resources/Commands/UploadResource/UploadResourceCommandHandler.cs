using MediatR;
using Microsoft.EntityFrameworkCore;
using QuranInstitute.Application.Common.Interfaces;
using QuranInstitute.Application.Common.Models;
using QuranInstitute.Domain.Entities.Education;
using QuranInstitute.Domain.Enums;

namespace QuranInstitute.Application.Features.Resources.Commands.UploadResource;

public class UploadResourceCommandHandler
    : IRequestHandler<UploadResourceCommand, Result<Guid>>
{
    private readonly IApplicationDbContext _db;
    private readonly IFileStorageService _storage;
    private readonly ICurrentUserService _current;

    public UploadResourceCommandHandler(
        IApplicationDbContext db,
        IFileStorageService storage,
        ICurrentUserService current)
    {
        _db = db;
        _storage = storage;
        _current = current;
    }

    public async Task<Result<Guid>> Handle(
        UploadResourceCommand request, CancellationToken ct)
    {
        var clsExists = await _db.Classes
            .AnyAsync(c => c.Id == request.ClassId && !c.IsDeleted, ct);

        if (!clsExists)
            return Result<Guid>.Failure("کلاس یافت نشد.");

        if (request.SessionId.HasValue)
        {
            var sessionExists = await _db.ClassSessions
                .AnyAsync(s => s.Id == request.SessionId.Value
                            && s.ClassId == request.ClassId, ct);

            if (!sessionExists)
                return Result<Guid>.Failure("جلسه یافت نشد یا به این کلاس تعلق ندارد.");
        }

        string? fileUrl = null;
        long? fileSize = request.FileSize;
        string? mimeType = request.ContentType;

        // ─── آپلود فایل (اگر فایل است) ───
        if (request.Type != ResourceType.Link && request.FileStream != null)
        {
            try
            {
                fileUrl = await _storage.UploadAsync(
                    request.FileStream,
                    request.FileName!,
                    request.ContentType ?? "application/octet-stream",
                    ct);

                fileSize = request.FileStream.Length;
            }
            catch (Exception ex)
            {
                return Result<Guid>.Failure($"خطا در آپلود فایل: {ex.Message}");
            }
        }
        else if (request.Type == ResourceType.Link)
        {
            fileUrl = request.ExternalUrl!.Trim();
        }

        if (string.IsNullOrWhiteSpace(fileUrl))
            return Result<Guid>.Failure("آدرس فایل/لینک نامعتبر است.");

        var resource = new ClassResource
        {
            ClassId = request.ClassId,
            SessionId = request.SessionId,
            Type = request.Type,
            Title = request.Title.Trim(),
            Description = request.Description?.Trim(),
            FileUrl = fileUrl,
            FileSize = fileSize,
            MimeType = mimeType,
            EnrolledOnly = request.EnrolledOnly,
            UploadedBy = _current.UserId ?? Guid.Empty
        };

        _db.ClassResources.Add(resource);
        await _db.SaveChangesAsync(ct);

        return Result<Guid>.Success(resource.Id);
    }
}