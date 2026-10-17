using Microsoft.AspNetCore.Mvc.Rendering;
using QuranInstitute.Application.Common.Models;
using QuranInstitute.Application.Features.Payments.Queries.GetPaymentsList;
using QuranInstitute.Domain.Enums;

namespace QuranInstitute.Web.ViewModels.Payments;

public class PaymentListViewModel
{
    public PagedList<PaymentListItemDto> Payments { get; set; } = new();
    public string? Search { get; set; }
    public PaymentMethod? Method { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }

    public List<SelectListItem> MethodOptions { get; set; } = new();

    // برای فرم ثبت پرداخت
    public CreatePaymentViewModel Form { get; set; } = new();
}