using System.ComponentModel.DataAnnotations;

namespace QuranInstitute.Web.ViewModels.Admin;

public class ResetPasswordViewModel
{
    public Guid UserId { get; set; }
    public string UserFullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "رمز جدید الزامی است.")]
    [MinLength(6, ErrorMessage = "رمز جدید حداقل ۶ کاراکتر باشد.")]
    [DataType(DataType.Password)]
    [Display(Name = "رمز جدید")]
    public string NewPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "تکرار رمز الزامی است.")]
    [Compare(nameof(NewPassword), ErrorMessage = "تکرار رمز مطابقت ندارد.")]
    [DataType(DataType.Password)]
    [Display(Name = "تکرار رمز جدید")]
    public string ConfirmPassword { get; set; } = string.Empty;
}