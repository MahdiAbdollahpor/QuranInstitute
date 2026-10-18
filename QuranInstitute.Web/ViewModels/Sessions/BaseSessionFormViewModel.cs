using System.ComponentModel.DataAnnotations;

namespace QuranInstitute.Web.ViewModels.Sessions;

public abstract class BaseSessionFormViewModel
{
    public Guid Id { get; set; }
    public Guid ClassId { get; set; }
    public string ClassName { get; set; } = string.Empty;

    [Required(ErrorMessage = "شماره جلسه الزامی است.")]
    [Range(1, 10000, ErrorMessage = "شماره جلسه باید بین ۱ تا ۱۰۰۰۰ باشد.")]
    [Display(Name = "شماره جلسه")]
    public int SessionNumber { get; set; }

    [Required(ErrorMessage = "تاریخ جلسه الزامی است.")]
    [Display(Name = "تاریخ جلسه")]
    public DateTime Date { get; set; } = DateTime.Today;

    [Required(ErrorMessage = "عنوان جلسه الزامی است.")]
    [MaxLength(200)]
    [Display(Name = "عنوان/موضوع جلسه")]
    public string Title { get; set; } = string.Empty;

    [MaxLength(2000)]
    [Display(Name = "توضیحات")]
    public string? Description { get; set; }

    [Display(Name = "جلسه آنلاین است")]
    public bool IsOnline { get; set; }

    [MaxLength(1000)]
    [Display(Name = "لینک جلسه آنلاین")]
    public string? OnlineLink { get; set; }

    [MaxLength(1000)]
    [Display(Name = "لینک فیلم ضبط‌شده")]
    public string? RecordingUrl { get; set; }
}