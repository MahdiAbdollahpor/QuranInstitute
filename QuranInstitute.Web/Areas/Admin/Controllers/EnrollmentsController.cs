using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using QuranInstitute.Application.Features.Classes.Queries.GetClassesList;
using QuranInstitute.Application.Features.Enrollments.Commands.ApproveEnrollment;
using QuranInstitute.Application.Features.Enrollments.Commands.CancelEnrollment;
using QuranInstitute.Application.Features.Enrollments.Commands.RejectEnrollment;
using QuranInstitute.Application.Features.Enrollments.Commands.TransferStudent;
using QuranInstitute.Application.Features.Enrollments.Queries.GetClassStudents;
using QuranInstitute.Application.Features.Enrollments.Queries.GetEnrollmentsList;
using QuranInstitute.Domain.Constants;
using QuranInstitute.Domain.Enums;
using QuranInstitute.Web.ViewModels.Enrollments;

namespace QuranInstitute.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = $"{Roles.Admin},{Roles.Teacher}")]
public class EnrollmentsController : MediatorControllerBase
{
    public EnrollmentsController(IMediator mediator) : base(mediator) { }

    // ─────────────── INDEX ───────────────
    [HttpGet]
    public async Task<IActionResult> Index(
        string? search = null,
        Guid? classId = null,
        EnrollmentStatus? status = null,
        int page = 1)
    {
        ViewData["Title"] = "ثبت‌نام‌ها";

        var result = await Mediator.Send(new GetEnrollmentsListQuery
        {
            Search = search,
            ClassId = classId,
            Status = status,
            Page = page,
            PageSize = 20
        });

        var classes = await Mediator.Send(new GetClassesListQuery
        {
            ActiveOnly = false,
            PageSize = 500
        });

        var vm = new EnrollmentListViewModel
        {
            Enrollments = result,
            Search = search,
            ClassId = classId,
            Status = status,
            Classes = classes.Items.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name,
                Selected = classId == c.Id
            }).ToList(),
            StatusOptions = BuildStatusOptions(status)
        };

        return View(vm);
    }

    // ─────────────── CLASS STUDENTS ───────────────
    [HttpGet]
    public async Task<IActionResult> ClassStudents(Guid id)
    {
        var result = await Mediator.Send(new GetClassStudentsQuery(id));

        if (!result.IsSuccess)
        {
            TempData["Error"] = result.Error;
            return RedirectToAction(nameof(Index));
        }

        ViewData["Title"] = $"شاگردان کلاس {result.Value!.ClassName}";
        return View(new ClassStudentsViewModel { Data = result.Value! });
    }

    // ─────────────── APPROVE ───────────────
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Approve(Guid id, string? returnUrl = null)
    {
        var result = await Mediator.Send(new ApproveEnrollmentCommand(id));

        TempData[result.IsSuccess ? "Success" : "Error"] =
            result.IsSuccess ? "ثبت‌نام تأیید شد." : result.Error;

        return RedirectBack(returnUrl);
    }

    // ─────────────── REJECT ───────────────
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Reject(Guid id, string? reason = null, string? returnUrl = null)
    {
        var result = await Mediator.Send(new RejectEnrollmentCommand(id, reason));

        TempData[result.IsSuccess ? "Success" : "Error"] =
            result.IsSuccess ? "ثبت‌نام رد شد." : result.Error;

        return RedirectBack(returnUrl);
    }

    // ─────────────── CANCEL ───────────────
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancel(Guid id, string? returnUrl = null)
    {
        var result = await Mediator.Send(new CancelEnrollmentCommand(id));

        TempData[result.IsSuccess ? "Success" : "Error"] =
            result.IsSuccess ? "ثبت‌نام لغو شد." : result.Error;

        return RedirectBack(returnUrl);
    }

    // ─────────────── TRANSFER (GET) ───────────────
    [HttpGet]
    public async Task<IActionResult> Transfer(Guid studentId, Guid fromClassId)
    {
        // اینجا برای سادگی، مستقیم داده را از Handler کلاس‌ها می‌گیریم
        // در گام‌های بعدی می‌توانیم یک Query اختصاصی برای این کار بسازیم
        var classes = await Mediator.Send(new GetClassesListQuery
        {
            ActiveOnly = true,
            PageSize = 500
        });

        ViewData["Title"] = "انتقال شاگرد";

        var vm = new TransferStudentViewModel
        {
            StudentId = studentId,
            FromClassId = fromClassId,
            AvailableClasses = classes.Items
                .Where(c => c.Id != fromClassId)
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = $"{c.Name} ({c.StudentCount}/{c.Capacity})"
                }).ToList()
        };

        return View(vm);
    }

    // ─────────────── TRANSFER (POST) ───────────────
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Transfer(TransferStudentViewModel vm)
    {
        if (!vm.ToClassId.HasValue)
        {
            ModelState.AddModelError(nameof(vm.ToClassId), "کلاس مقصد را انتخاب کنید.");
        }

        if (!ModelState.IsValid)
        {
            var classes = await Mediator.Send(new GetClassesListQuery
            {
                ActiveOnly = true,
                PageSize = 500
            });

            vm.AvailableClasses = classes.Items
                .Where(c => c.Id != vm.FromClassId)
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = $"{c.Name} ({c.StudentCount}/{c.Capacity})"
                }).ToList();

            return View(vm);
        }

        var result = await Mediator.Send(new TransferStudentCommand
        {
            StudentId = vm.StudentId,
            FromClassId = vm.FromClassId,
            ToClassId = vm.ToClassId!.Value
        });

        if (!result.IsSuccess)
        {
            ModelState.AddModelError("", result.Error!);

            var classes = await Mediator.Send(new GetClassesListQuery
            {
                ActiveOnly = true,
                PageSize = 500
            });

            vm.AvailableClasses = classes.Items
                .Where(c => c.Id != vm.FromClassId)
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = $"{c.Name} ({c.StudentCount}/{c.Capacity})"
                }).ToList();

            return View(vm);
        }

        TempData["Success"] = "شاگرد با موفقیت منتقل شد.";
        return RedirectToAction(nameof(ClassStudents), new { id = vm.ToClassId });
    }

    // ─────────────── HELPERS ───────────────
    private IActionResult RedirectBack(string? returnUrl)
    {
        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            return Redirect(returnUrl);

        return RedirectToAction(nameof(Index));
    }

    private static List<SelectListItem> BuildStatusOptions(EnrollmentStatus? selected)
    {
        var list = new List<SelectListItem>
        {
            new() { Value = "", Text = "همه وضعیت‌ها", Selected = !selected.HasValue },
            new() { Value = ((int)EnrollmentStatus.Pending).ToString(),
                    Text = "در انتظار",
                    Selected = selected == EnrollmentStatus.Pending },
            new() { Value = ((int)EnrollmentStatus.Approved).ToString(),
                    Text = "تأییدشده",
                    Selected = selected == EnrollmentStatus.Approved },
            new() { Value = ((int)EnrollmentStatus.Rejected).ToString(),
                    Text = "ردشده",
                    Selected = selected == EnrollmentStatus.Rejected },
            new() { Value = ((int)EnrollmentStatus.Canceled).ToString(),
                    Text = "لغوشده",
                    Selected = selected == EnrollmentStatus.Canceled },
        };

        return list;
    }
}