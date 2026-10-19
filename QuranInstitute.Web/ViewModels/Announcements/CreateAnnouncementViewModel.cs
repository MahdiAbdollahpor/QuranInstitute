using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace QuranInstitute.Web.ViewModels.Announcements;

public class CreateAnnouncementViewModel
{
    [Required(ErrorMessage = "عنوان الزامی است.")]
    [MaxLength(200)]
    [Display(Name = "عنوان")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "متن پیام الزامی است.")]
    [MaxLength(4000)]
    [Display(Name = "متن پیام")]
    public string Body { get; set; } = string.Empty;

    [Display(Name = "کلاس هدف")]
    public Guid? TargetClassId { get; set; }

    [Display(Name = "شاگرد هدف")]
    public Guid? TargetStudentId { get; set; }

    [Display(Name = "ارسال درون‌برنامه")]
    public bool SendInApp { get; set; } = true;

    [Display(Name = "ارسال SMS")]
    public bool SendSms { get; set; }

    [Display(Name = "ارسال WhatsApp")]
    public bool SendWhatsApp { get; set; }

    public List<SelectListItem> Classes { get; set; } = new();
    public List<SelectListItem> Students { get; set; } = new();
}