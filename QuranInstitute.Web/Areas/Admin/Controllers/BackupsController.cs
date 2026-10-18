using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuranInstitute.Application.Common.Interfaces;
using QuranInstitute.Domain.Constants;
using QuranInstitute.Web.ViewModels.Backups;

namespace QuranInstitute.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = Roles.Admin)]
public class BackupsController : Controller
{
    private readonly IBackupService _backupService;
    private readonly ILogger<BackupsController> _logger;

    public BackupsController(
        IBackupService backupService,
        ILogger<BackupsController> logger)
    {
        _backupService = backupService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        ViewData["Title"] = "پشتیبان‌گیری";

        var files = await _backupService.ListBackupsAsync();

        return View(new BackupsIndexViewModel { Files = files });
    }

    // ─── دانلود پشتیبان جدید ───
    [HttpGet]
    public async Task<IActionResult> Download()
    {
        try
        {
            var bytes = await _backupService.CreateBackupAsync();
            var fileName = $"backup-{DateTime.Now:yyyyMMdd-HHmmss}.zip";

            return File(bytes, "application/zip", fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Backup failed");
            TempData["Error"] = "خطا در ساخت پشتیبان: " + ex.Message;
            return RedirectToAction(nameof(Index));
        }
    }

    // ─── دانلود یک فایل پشتیبان موجود ───
    [HttpGet]
    public async Task<IActionResult> DownloadExisting(string fileName)
    {
        var files = await _backupService.ListBackupsAsync();
        if (!files.Any(f => f.FileName == fileName))
            return NotFound();

        var path = Path.Combine(
            Directory.GetCurrentDirectory(), "Backups", fileName);

        // ⚠️ اگر ContentRootPath استفاده می‌کنی، از مسیر درست استفاده کن
        var contentRoot = HttpContext.RequestServices
            .GetRequiredService<IHostEnvironment>().ContentRootPath;
        path = Path.Combine(contentRoot, "Backups", fileName);

        if (!System.IO.File.Exists(path))
            return NotFound();

        var bytes = await System.IO.File.ReadAllBytesAsync(path);
        return File(bytes, "application/zip", fileName);
    }

    // ─── بازیابی ───
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Restore(IFormFile file)
    {
        if (file is null || file.Length == 0)
        {
            TempData["Error"] = "فایلی انتخاب نشده است.";
            return RedirectToAction(nameof(Index));
        }

        try
        {
            using var stream = file.OpenReadStream();
            var ok = await _backupService.RestoreBackupAsync(stream);

            if (ok)
                TempData["Success"] = "بازیابی با موفقیت انجام شد.";
            else
                TempData["Warning"] = "بازیابی فعلاً پیاده‌سازی نشده است. برای امنیت، به پشتیبان دستی مراجعه کنید.";

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            TempData["Error"] = "خطا در بازیابی: " + ex.Message;
            return RedirectToAction(nameof(Index));
        }
    }
}