using Microsoft.Extensions.Hosting;
using QuranInstitute.Application.Common.Interfaces;

namespace QuranInstitute.Infrastructure.Services.FileStorage;

/// <summary>
/// پیاده‌سازی ساده ذخیره فایل روی دیسک.
/// در production می‌توان با S3FileStorageService جایگزین کرد.
/// </summary>
public class LocalFileStorageService : IFileStorageService
{
    private readonly IHostEnvironment _env;
    private const string RootFolder = "uploads";

    public LocalFileStorageService(IHostEnvironment env) => _env = env;

    public async Task<string> UploadAsync(
        Stream stream, string fileName, string contentType, CancellationToken ct = default)
    {
        var folder = Path.Combine(_env.ContentRootPath, "wwwroot", RootFolder);
        Directory.CreateDirectory(folder);

        var safeName = $"{Guid.NewGuid():N}_{Path.GetFileName(fileName)}";
        var fullPath = Path.Combine(folder, safeName);

        await using var fs = File.Create(fullPath);
        await stream.CopyToAsync(fs, ct);

        return $"/{RootFolder}/{safeName}";
    }

    public Task DeleteAsync(string fileUrl, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(fileUrl)) return Task.CompletedTask;

        var rel = fileUrl.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
        var full = Path.Combine(_env.ContentRootPath, "wwwroot", rel);

        if (File.Exists(full)) File.Delete(full);
        return Task.CompletedTask;
    }

    public string GetPublicUrl(string fileUrl) => fileUrl;
}