using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using QuranInstitute.Application.Common.Interfaces;
using QuranInstitute.Shared.Helpers;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace QuranInstitute.Infrastructure.Services.Sms;

public class KavenegarSmsService : IRawSmsService
{
    private readonly HttpClient _http;
    private readonly KavenegarOptions _options;
    private readonly ILogger<KavenegarSmsService> _logger;

    public KavenegarSmsService(
        HttpClient http,
        IOptions<KavenegarOptions> options,
        ILogger<KavenegarSmsService> logger)
    {
        _http = http;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<bool> SendAsync(string mobile, string message, CancellationToken ct = default)
    {
        var result = await SendBulkAsync(new[] { mobile }, message, ct);
        return result > 0;
    }

    public async Task<int> SendBulkAsync(
        IEnumerable<string> mobiles, string message, CancellationToken ct = default)
    {
        if (!_options.Enabled)
        {
            _logger.LogWarning("Kavenegar غیرفعال است. ارسال انجام نشد.");
            return 0;
        }

        if (string.IsNullOrWhiteSpace(_options.ApiKey))
        {
            _logger.LogError("Kavenegar:ApiKey خالی است.");
            return 0;
        }

        var recipients = mobiles
            .Select(NormalizeMobile)
            .Where(m => !string.IsNullOrEmpty(m))
            .Distinct()
            .ToList();

        if (!recipients.Any())
            return 0;

        try
        {
            var url = $"{_options.BaseUrl}/{_options.ApiKey}/sms/send.json";

            var form = new Dictionary<string, string>
            {
                { "receptor", string.Join(",", recipients) },
                { "message", message },
                { "sender", _options.SenderNumber }
            };

            using var content = new FormUrlEncodedContent(form);
            var response = await _http.PostAsync(url, content, ct);
            response.EnsureSuccessStatusCode();

            var result = await response.Content
                .ReadFromJsonAsync<KavenegarResponse>(cancellationToken: ct);

            if (result?.Return?.Status == 200)
            {
                _logger.LogInformation("Kavenegar: {Count} پیام ارسال شد.", recipients.Count);
                return recipients.Count;
            }

            _logger.LogWarning("Kavenegar error: {Code} {Message}",
                result?.Return?.Status, result?.Return?.Message);
            return 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Kavenegar exception");
            return 0;
        }
    }

    private static string NormalizeMobile(string mobile)
    {
        var n = mobile.ConvertToEn().Trim().Replace(" ", "").Replace("-", "");
        if (n.StartsWith("+98")) n = "0" + n.Substring(3);
        if (n.StartsWith("0098")) n = "0" + n.Substring(4);
        if (n.StartsWith("98") && n.Length == 12) n = "0" + n.Substring(2);
        return n;
    }
}

public class KavenegarOptions
{
    public bool Enabled { get; set; }
    public string ApiKey { get; set; } = string.Empty;
    public string SenderNumber { get; set; } = string.Empty;
    public string BaseUrl { get; set; } = "https://api.kavenegar.com/v1";
}

internal class KavenegarResponse
{
    [JsonPropertyName("return")] public KavenegarReturn? Return { get; set; }
}

internal class KavenegarReturn
{
    [JsonPropertyName("status")] public int Status { get; set; }
    [JsonPropertyName("message")] public string? Message { get; set; }
}