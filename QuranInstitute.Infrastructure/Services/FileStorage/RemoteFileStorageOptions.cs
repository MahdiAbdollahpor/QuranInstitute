namespace QuranInstitute.Infrastructure.Services.FileStorage;

public class RemoteFileStorageOptions
{
    public bool Enabled { get; set; }
    public string BaseUrl { get; set; } = string.Empty;
    public string ApiKey { get; set; } = string.Empty;
    public string UploadEndpoint { get; set; } = string.Empty;
    public string PublicBaseUrl { get; set; } = string.Empty;
    public int TimeoutSeconds { get; set; } = 60;
}