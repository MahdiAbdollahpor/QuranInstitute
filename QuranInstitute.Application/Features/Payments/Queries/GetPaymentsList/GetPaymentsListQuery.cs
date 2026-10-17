using MediatR;
using QuranInstitute.Application.Common.Models;
using QuranInstitute.Domain.Enums;

namespace QuranInstitute.Application.Features.Payments.Queries.GetPaymentsList;

public class GetPaymentsListQuery : IRequest<PagedList<PaymentListItemDto>>
{
    public string? Search { get; set; }
    public Guid? StudentId { get; set; }
    public PaymentMethod? Method { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}