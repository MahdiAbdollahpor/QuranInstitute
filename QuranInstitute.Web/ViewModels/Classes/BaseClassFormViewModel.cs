using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using QuranInstitute.Domain.Enums;

namespace QuranInstitute.Web.ViewModels.Classes;

public abstract class BaseClassFormViewModel
{

    public Guid Id { get; set; }
    [Required(ErrorMessage = "نام کلاس الزامی است.")]
    [Display(Name = "نام کلاس")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "نام استاد الزامی است.")]
    [Display(Name = "استاد")]
    public string TeacherName { get; set; } = string.Empty;

    [Display(Name = "ساعت کلاس")]
    public string? ClassTime { get; set; }

    [Display(Name = "روزهای برگزاری")]
    public List<int> SelectedDays { get; set; } = new();

    [Display(Name = "نوع کلاس")]
    public ClassType Type { get; set; } = ClassType.InPerson;

    [Display(Name = "لینک کلاس آنلاین")]
    public string? DefaultOnlineLink { get; set; }

    [Display(Name = "ظرفیت")]
    public int Capacity { get; set; } = 30;

    // ─── فقط برای View ───
    public List<SelectListItem> DayOptions { get; set; } = new();
    public List<SelectListItem> TypeOptions { get; set; } = new();
}