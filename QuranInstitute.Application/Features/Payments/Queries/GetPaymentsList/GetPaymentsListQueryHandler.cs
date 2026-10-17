using MediatR;
using Microsoft.EntityFrameworkCore;
using QuranInstitute.Application.Common.Interfaces;
using QuranInstitute.Application.Common.Models;
using QuranInstitute.Domain.Enums;

namespace QuranInstitute.Application.Features.Payments.Queries.GetPaymentsList;

public class GetPaymentsListQueryHandler
    : IRequestHandler<GetPaymentsListQuery, PagedList<PaymentListItemDto>>
{
    private readonly IApplicationDbContext _db;

    public GetPaymentsListQueryHandler(IApplicationDbContext db) => _db = db;

    public async Task<PagedList<PaymentListItemDto>> Handle(
        GetPaymentsListQuery request, CancellationToken ct)
    {
        var q = _db.Payments
            .AsNoTracking()
            .Include(p => p.Student)
            .AsQueryable();

        if (request.StudentId.HasValue)
            q = q.Where(p => p.StudentId == request.StudentId.Value);

        if (request.Method.HasValue)
            q = q.Where(p => p.Method == request.Method.Value);

        if (request.FromDate.HasValue)
            q = q.Where(p => p.Date >= request.FromDate.Value.Date);

        if (request.ToDate.HasValue)
            q = q.Where(p => p.Date <= request.ToDate.Value.Date);

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var s = request.Search.Trim();
            q = q.Where(p =>
                p.Student.Name.Contains(s) ||
                p.Student.ParentMobile.Contains(s) ||
                p.ReceiptNo.Contains(s));
        }

        var total = await q.CountAsync(ct);

        var items = await q
            .OrderByDescending(p => p.Date)
            .ThenByDescending(p => p.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(p => new PaymentListItemDto
            {
                Id = p.Id,
                StudentId = p.StudentId,
                StudentName = p.Student.Name,
                ClassName = p.Student.Enrollments
                    .Where(e => e.Status == EnrollmentStatus.Approved)
                    .Select(e => e.Class.Name)
                    .FirstOrDefault() ?? "—",
                Amount = p.Amount,
                Date = p.Date,
                Method = p.Method,
                Status = p.Status,
                ReceiptNo = p.ReceiptNo,
                Note = p.Note
            })
            .ToListAsync(ct);

        return new PagedList<PaymentListItemDto>
        {
            Items = items,
            TotalCount = total,
            Page = request.Page,
            PageSize = request.PageSize
        };
    }
}