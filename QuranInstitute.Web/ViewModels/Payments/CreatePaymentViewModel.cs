using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using QuranInstitute.Domain.Enums;

namespace QuranInstitute.Web.ViewModels.Payments;

public class CreatePaymentViewModel
{
    [Required(ErrorMessage = "شاگرد را انتخاب کنید.")]
    [Display(Name = "شاگرد")]
    public Guid? StudentId { get; set; }

    [Required(ErrorMessage = "مبلغ را وارد کنید.")]
    [Display(Name = "مبلغ (تومان)")]
    public decimal? Amount { get; set; }

    [Required(ErrorMessage = "تاریخ پرداخت الزامی است.")]
    [Display(Name = "تاریخ پرداخت")]
    public DateTime Date { get; set; } = DateTime.Today;

    [Display(Name = "روش پرداخت")]
    public PaymentMethod Method { get; set; } = PaymentMethod.Cash;

    [Display(Name = "توضیحات")]
    public string? Note { get; set; }

    // ─── برای View ───
    public List<SelectListItem> Students { get; set; } = new();
    public List<SelectListItem> MethodOptions { get; set; } = new();
}