namespace QuranInstitute.Application.Common.Interfaces;

public interface IFileStorageService
{
    Task<string> UploadAsync(Stream stream, string fileName, string contentType, CancellationToken ct = default);
    Task DeleteAsync(string fileUrl, CancellationToken ct = default);
    string GetPublicUrl(string fileUrl);
}