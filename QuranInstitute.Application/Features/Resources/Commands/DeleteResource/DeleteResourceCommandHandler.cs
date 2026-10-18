using MediatR;
using Microsoft.EntityFrameworkCore;
using QuranInstitute.Application.Common.Interfaces;
using QuranInstitute.Application.Common.Models;
using QuranInstitute.Domain.Enums;

namespace QuranInstitute.Application.Features.Resources.Commands.DeleteResource;

public class DeleteResourceCommandHandler
    : IRequestHandler<DeleteResourceCommand, Result>
{
    private readonly IApplicationDbContext _db;
    private readonly IFileStorageService _storage;

    public DeleteResourceCommandHandler(
        IApplicationDbContext db,
        IFileStorageService storage)
    {
        _db = db;
        _storage = storage;
    }

    public async Task<Result> Handle(
        DeleteResourceCommand request, CancellationToken ct)
    {
        var resource = await _db.ClassResources
            .FirstOrDefaultAsync(r => r.Id == request.Id, ct);

        if (resource is null)
            return Result.Failure("منبع یافت نشد.");

        // حذف فایل از Storage (اگر Link نباشد)
        if (resource.Type != ResourceType.Link
            && !string.IsNullOrWhiteSpace(resource.FileUrl))
        {
            try
            {
                await _storage.DeleteAsync(resource.FileUrl, ct);
            }
            catch
            {
                // بی‌اهمیت — DB را پاک می‌کنیم حتی اگر فایل حذف نشد
            }
        }

        _db.ClassResources.Remove(resource);
        await _db.SaveChangesAsync(ct);
        return Result.Success();
    }
}