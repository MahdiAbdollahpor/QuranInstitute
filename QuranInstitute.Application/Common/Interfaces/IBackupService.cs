using QuranInstitute.Application.Features.Backups;

namespace QuranInstitute.Application.Common.Interfaces;

public interface IBackupService
{
    Task<byte[]> CreateBackupAsync(CancellationToken ct = default);
    Task<List<BackupFileDto>> ListBackupsAsync(CancellationToken ct = default);
    Task<bool> RestoreBackupAsync(Stream backupStream, CancellationToken ct = default);
}