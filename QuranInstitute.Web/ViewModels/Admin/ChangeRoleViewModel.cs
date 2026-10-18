using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace QuranInstitute.Web.ViewModels.Admin;

public class ChangeRoleViewModel
{
    public Guid UserId { get; set; }
    public string UserFullName { get; set; } = string.Empty;
    public string CurrentRole { get; set; } = string.Empty;

    [Required(ErrorMessage = "نقش جدید را انتخاب کنید.")]
    [Display(Name = "نقش جدید")]
    public string NewRole { get; set; } = string.Empty;

    public List<SelectListItem> RoleOptions { get; set; } = new();
}