using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using QuranInstitute.Application.Features.Payments.Commands.DeletePayment;
using QuranInstitute.Application.Features.Payments.Commands.RecordManualPayment;
using QuranInstitute.Application.Features.Payments.Queries.GetPaymentsList;
using QuranInstitute.Application.Features.Payments.Queries.GetReceiptData;
using QuranInstitute.Application.Features.Students.Queries.GetStudentsList;
using QuranInstitute.Domain.Constants;
using QuranInstitute.Domain.Enums;
using QuranInstitute.Web.ViewModels.Payments;

namespace QuranInstitute.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = $"{Roles.Admin},{Roles.Teacher}")]
public class PaymentsController : MediatorControllerBase
{
    public PaymentsController(IMediator mediator) : base(mediator) { }

    // ─────────────── INDEX ───────────────
    [HttpGet]
    public async Task<IActionResult> Index(
        string? search = null,
        PaymentMethod? method = null,
        DateTime? fromDate = null,
        DateTime? toDate = null,
        Guid? studentId = null,
        int page = 1)
    {
        ViewData["Title"] = "پرداخت‌ها";

        var result = await Mediator.Send(new GetPaymentsListQuery
        {
            Search = search,
            StudentId = studentId,
            Method = method,
            FromDate = fromDate,
            ToDate = toDate,
            Page = page,
            PageSize = 20
        });

        var students = await Mediator.Send(new GetStudentsListQuery
        {
            ActiveOnly = true,
            PageSize = 500
        });

        var vm = new PaymentListViewModel
        {
            Payments = result,
            Search = search,
            Method = method,
            FromDate = fromDate,
            ToDate = toDate,
            MethodOptions = BuildMethodOptions(method),
            Form = new CreatePaymentViewModel
            {
                Date = DateTime.Today,
                Students = students.Items.Select(s => new SelectListItem
                {
                    Value = s.Id.ToString(),
                    Text = $"{s.Name} ({s.ClassName})",
                    Selected = studentId == s.Id
                }).ToList(),
                MethodOptions = BuildMethodOptions(PaymentMethod.Cash)
            }
        };

        return View(vm);
    }

    // ─────────────── CREATE (ثبت پرداخت) ───────────────
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
    [Bind(Prefix = "Form")] CreatePaymentViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            // ★ برای دیباگ: خطاها را ببین
            var errors = ModelState
                .Where(kv => kv.Value!.Errors.Any())
                .Select(kv => $"{kv.Key}: {string.Join(", ", kv.Value!.Errors.Select(e => e.ErrorMessage))}")
                .ToList();

            TempData["Error"] = "اطلاعات فرم نامعتبر است: " + string.Join(" | ", errors);
            return RedirectToAction(nameof(Index));
        }

        var result = await Mediator.Send(new RecordManualPaymentCommand
        {
            StudentId = vm.StudentId!.Value,
            Amount = vm.Amount!.Value,
            Date = vm.Date,
            Method = vm.Method,
            Note = vm.Note
        });

        if (!result.IsSuccess)
        {
            TempData["Error"] = result.Error;
            return RedirectToAction(nameof(Index));
        }

        TempData["Success"] = "پرداخت با موفقیت ثبت شد.";

        // ریدایرکت به رسید
        return RedirectToAction(nameof(Receipt), new { id = result.Value });
    }

    // ─────────────── RECEIPT ───────────────
    [HttpGet]
    public async Task<IActionResult> Receipt(Guid id)
    {
        var result = await Mediator.Send(new GetReceiptDataQuery(id));

        if (!result.IsSuccess)
        {
            TempData["Error"] = result.Error;
            return RedirectToAction(nameof(Index));
        }

        ViewData["Title"] = "رسید پرداخت";
        return View(result.Value!);
    }

    // ─────────────── DELETE ───────────────
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await Mediator.Send(new DeletePaymentCommand(id));

        TempData[result.IsSuccess ? "Success" : "Error"] =
            result.IsSuccess ? "پرداخت حذف شد." : result.Error;

        return RedirectToAction(nameof(Index));
    }

    // ─────────────── HELPERS ───────────────
    private static List<SelectListItem> BuildMethodOptions(PaymentMethod? selected = null)
    {
        return new List<SelectListItem>
        {
            new() { Value = "", Text = "همه روش‌ها", Selected = !selected.HasValue },
            new() { Value = ((int)PaymentMethod.Cash).ToString(), Text = "نقدی",
                    Selected = selected == PaymentMethod.Cash },
            new() { Value = ((int)PaymentMethod.Card).ToString(), Text = "کارت به کارت",
                    Selected = selected == PaymentMethod.Card },
            new() { Value = ((int)PaymentMethod.Online).ToString(), Text = "آنلاین",
                    Selected = selected == PaymentMethod.Online },
            new() { Value = ((int)PaymentMethod.Other).ToString(), Text = "سایر",
                    Selected = selected == PaymentMethod.Other }
        };
    }
}