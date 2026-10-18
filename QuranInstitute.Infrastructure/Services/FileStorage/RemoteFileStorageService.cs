using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using QuranInstitute.Application.Common.Interfaces;

namespace QuranInstitute.Infrastructure.Services.FileStorage;

/// <summary>
/// ذخیره فایل روی هاست جداگانه.
/// فعلاً غیرفعال است (Enabled = false).
/// وقتی هاست خریداری شد، در appsettings.json پر کن و Enabled را true کن.
/// </summary>
public class RemoteFileStorageService : IFileStorageService
{
    private readonly HttpClient _http;
    private readonly RemoteFileStorageOptions _options;
    private readonly ILogger<RemoteFileStorageService> _logger;

    public RemoteFileStorageService(
        HttpClient http,
        IOptions<RemoteFileStorageOptions> options,
        ILogger<RemoteFileStorageService> logger)
    {
        _http = http;
        _options = options.Value;
        _logger = logger;

        if (_options.TimeoutSeconds > 0)
            _http.Timeout = TimeSpan.FromSeconds(_options.TimeoutSeconds);
    }

    public async Task<string> UploadAsync(
        Stream stream, string fileName, string contentType, CancellationToken ct = default)
    {
        if (!_options.Enabled)
            throw new InvalidOperationException(
                "RemoteFileStorage غیرفعال است. در appsettings.json مقدار Enabled را true کنید.");

        if (string.IsNullOrWhiteSpace(_options.UploadEndpoint))
            throw new InvalidOperationException(
                "RemoteFileStorage:UploadEndpoint خالی است.");

        using var content = new MultipartFormDataContent();
        using var fileContent = new StreamContent(stream);
        fileContent.Headers.ContentType = new MediaTypeHeaderValue(contentType);
        content.Add(fileContent, "file", fileName);

        using var req = new HttpRequestMessage(HttpMethod.Post, _options.UploadEndpoint);
        if (!string.IsNullOrWhiteSpace(_options.ApiKey))
            req.Headers.Add("X-Api-Key", _options.ApiKey);
        req.Content = content;

        var response = await _http.SendAsync(req, ct);
        response.EnsureSuccessStatusCode();

        var result = await response.Content
            .ReadFromJsonAsync<RemoteUploadResult>(cancellationToken: ct);

        if (string.IsNullOrWhiteSpace(result?.Url))
            throw new InvalidOperationException("پاسخ هاست فایل نامعتبر است.");

        _logger.LogInformation("File uploaded to remote: {Url}", result.Url);

        return result.Url;
    }

    public Task DeleteAsync(string fileUrl, CancellationToken ct = default)
    {
        if (!_options.Enabled) return Task.CompletedTask;

        // TODO: بعد از دریافت مستندات API هاست، پیاده‌سازی کن
        _logger.LogInformation("Remote delete requested for: {Url}", fileUrl);
        return Task.CompletedTask;
    }

    public string GetPublicUrl(string fileUrl)
    {
        if (string.IsNullOrWhiteSpace(fileUrl)) return string.Empty;
        if (fileUrl.StartsWith("http")) return fileUrl;

        var baseUrl = _options.PublicBaseUrl.TrimEnd('/');
        return $"{baseUrl}/{fileUrl.TrimStart('/')}";
    }
}

internal class RemoteUploadResult
{
    [JsonPropertyName("url")] public string? Url { get; set; }
    [JsonPropertyName("filename")] public string? Filename { get; set; }
    [JsonPropertyName("size")] public long? Size { get; set; }
}