using System.ComponentModel.DataAnnotations;

namespace QuranInstitute.Web.ViewModels.Settings;

public class SettingsViewModel
{
    [Required(ErrorMessage = "نام موسسه الزامی است.")]
    [MaxLength(200)]
    [Display(Name = "نام موسسه")]
    public string InstituteName { get; set; } = string.Empty;

    [MaxLength(500)]
    [Display(Name = "آدرس")]
    public string? Address { get; set; }

    [MaxLength(20)]
    [Display(Name = "تلفن")]
    public string? Phone { get; set; }

    [MaxLength(150)]
    [EmailAddress(ErrorMessage = "ایمیل معتبر نیست.")]
    [Display(Name = "ایمیل")]
    public string? Email { get; set; }

    [MaxLength(500)]
    [Display(Name = "آدرس لوگو")]
    public string? LogoUrl { get; set; }

    [MaxLength(2000)]
    [Display(Name = "قالب پیام غیبت")]
    public string AbsentTemplate { get; set; } = string.Empty;

    [MaxLength(2000)]
    [Display(Name = "قالب پیام بدهی")]
    public string DebtTemplate { get; set; } = string.Empty;

    [MaxLength(2000)]
    [Display(Name = "قالب اطلاعیه عمومی")]
    public string GeneralTemplate { get; set; } = string.Empty;
}