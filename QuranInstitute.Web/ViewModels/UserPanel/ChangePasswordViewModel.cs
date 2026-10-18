using System.ComponentModel.DataAnnotations;

namespace QuranInstitute.Web.ViewModels.UserPanel;

public class ChangePasswordViewModel
{
    [Required(ErrorMessage = "رمز فعلی الزامی است.")]
    [DataType(DataType.Password)]
    [Display(Name = "رمز فعلی")]
    public string CurrentPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "رمز جدید الزامی است.")]
    [MinLength(6, ErrorMessage = "رمز جدید حداقل ۶ کاراکتر باشد.")]
    [DataType(DataType.Password)]
    [Display(Name = "رمز جدید")]
    public string NewPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "تکرار رمز جدید الزامی است.")]
    [Compare(nameof(NewPassword), ErrorMessage = "تکرار رمز مطابقت ندارد.")]
    [DataType(DataType.Password)]
    [Display(Name = "تکرار رمز جدید")]
    public string ConfirmPassword { get; set; } = string.Empty;
}