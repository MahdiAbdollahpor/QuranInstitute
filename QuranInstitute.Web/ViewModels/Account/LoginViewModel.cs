using System.ComponentModel.DataAnnotations;

namespace QuranInstitute.Web.ViewModels.Account;

public class LoginViewModel
{
    [Required(ErrorMessage = "شماره موبایل را وارد کنید.")]
    [Display(Name = "شماره موبایل")]
    public string Mobile { get; set; } = string.Empty;

    [Required(ErrorMessage = "رمز عبور را وارد کنید.")]
    [DataType(DataType.Password)]
    [Display(Name = "رمز عبور")]
    public string Password { get; set; } = string.Empty;

    [Display(Name = "مرا به خاطر بسپار")]
    public bool RememberMe { get; set; }

    public string? ReturnUrl { get; set; }
}