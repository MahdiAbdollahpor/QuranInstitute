using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using QuranInstitute.Domain.Enums;

namespace QuranInstitute.Web.ViewModels.Resources;

public class UploadResourceViewModel
{
    public Guid ClassId { get; set; }
    public string ClassName { get; set; } = string.Empty;

    [Display(Name = "جلسه (اختیاری)")]
    public Guid? SessionId { get; set; }

    [Required(ErrorMessage = "نوع منبع را انتخاب کنید.")]
    [Display(Name = "نوع")]
    public ResourceType Type { get; set; } = ResourceType.Video;

    [Required(ErrorMessage = "عنوان الزامی است.")]
    [MaxLength(200)]
    [Display(Name = "عنوان")]
    public string Title { get; set; } = string.Empty;

    [MaxLength(2000)]
    [Display(Name = "توضیحات")]
    public string? Description { get; set; }

    [Display(Name = "فقط شاگردان ثبت‌نام‌شده")]
    public bool EnrolledOnly { get; set; } = true;

    // ★ برای آپلود فایل
    [Display(Name = "فایل")]
    public IFormFile? File { get; set; }

    // ★ برای لینک خارجی
    [Display(Name = "آدرس لینک")]
    public string? ExternalUrl { get; set; }

    // ─── فقط برای نمایش ───
    public List<SelectListItem> Sessions { get; set; } = new();
    public List<SelectListItem> TypeOptions { get; set; } = new();
}