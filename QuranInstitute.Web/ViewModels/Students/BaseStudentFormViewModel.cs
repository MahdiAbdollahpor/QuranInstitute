using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace QuranInstitute.Web.ViewModels.Students;

public abstract class BaseStudentFormViewModel
{
    public Guid Id { get; set; }
    [Required(ErrorMessage = "نام شاگرد الزامی است.")]
    [Display(Name = "نام و نام خانوادگی")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "نام ولی الزامی است.")]
    [Display(Name = "نام ولی")]
    public string ParentName { get; set; } = string.Empty;

    [Required(ErrorMessage = "شماره موبایل ولی الزامی است.")]
    [Display(Name = "شماره موبایل ولی")]
    public string ParentMobile { get; set; } = string.Empty;

    [Display(Name = "کلاس")]
    public Guid? ClassId { get; set; }

    [Display(Name = "شهریه ماهانه (تومان)")]
    public decimal MonthlyFee { get; set; }

    [Display(Name = "تاریخ شروع شهریه")]
    public DateTime StartDate { get; set; } = DateTime.Today;

    [Display(Name = "توضیحات")]
    public string? Note { get; set; }

    public List<SelectListItem> Classes { get; set; } = new();
}