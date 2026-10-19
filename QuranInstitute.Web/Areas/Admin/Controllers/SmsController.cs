using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using QuranInstitute.Application.Common.Interfaces;
using QuranInstitute.Application.Features.Classes.Queries.GetClassesList;
using QuranInstitute.Application.Features.Notifications.Commands.SendSms;
using QuranInstitute.Application.Features.Notifications.Queries.GetSmsHistory;
using QuranInstitute.Application.Features.Notifications.Queries.GetSmsStats;
using QuranInstitute.Application.Features.Notifications.Queries.PreviewAbsentSms;
using QuranInstitute.Application.Features.Notifications.Queries.PreviewDebtSms;
using QuranInstitute.Domain.Constants;
using QuranInstitute.Domain.Enums;
using QuranInstitute.Web.ViewModels.Sms;

namespace QuranInstitute.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = $"{Roles.Admin},{Roles.Teacher}")]
public class SmsController : MediatorControllerBase
{
    private readonly ISmsService _smsService;
    public SmsController(IMediator mediator, ISmsService smsService) : base(mediator)
    {
        _smsService = smsService;
    }

    // ─── صفحه اصلی SMS ───
    [HttpGet]
    public IActionResult Index()
    {
        ViewData["Title"] = "ارسال پیامک";
        return View();
    }

    // ─── پیامک غیبت ───
    [HttpGet]
    public async Task<IActionResult> SendAbsent(DateTime? date = null, Guid? classId = null)
    {
        var d = (date ?? DateTime.Today).Date;

        var result = await Mediator.Send(new PreviewAbsentSmsQuery
        {
            Date = d,
            ClassId = classId
        });

        var classes = await Mediator.Send(new GetClassesListQuery
        {
            ActiveOnly = true,
            PageSize = 500
        });

        ViewData["Title"] = "پیامک غیبت";

        return View(new AbsentSmsPreviewViewModel
        {
            Data = result.IsSuccess ? result.Value! : new(),
            SelectedDate = d,
            ClassId = classId,
            Classes = classes.Items.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name,
                Selected = classId == c.Id
            }).ToList()
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SendAbsentSms(
        DateTime date, Guid? classId, List<Guid> studentIds)
    {
        if (studentIds is null || !studentIds.Any())
        {
            TempData["Error"] = "هیچ شاگردی انتخاب نشده است.";
            return RedirectToAction(nameof(SendAbsent), new { date, classId });
        }

        // گرفتن پیام‌های رندر شده برای شاگردان انتخاب‌شده
        var preview = await Mediator.Send(new PreviewAbsentSmsQuery
        {
            Date = date,
            ClassId = classId
        });

        if (!preview.IsSuccess)
        {
            TempData["Error"] = preview.Error;
            return RedirectToAction(nameof(SendAbsent), new { date, classId });
        }

        var recipients = preview.Value!.Recipients
            .Where(r => studentIds.Contains(r.StudentId))
            .ToList();

        // ارسال تک‌تک (چون پیام‌ها شخصی‌سازی شده هستند)
        var smsRequests = recipients
    .Select(r => new SmsSendRequest
    {
        Mobile = r.Mobile,
        Message = r.RenderedMessage,
        Type = SmsType.Absent,
        StudentId = r.StudentId,
        RecipientName = r.ParentName
    })
    .ToList();

        var sent = await _smsService.SendBulkAsync(smsRequests);   // ← مستقیم از ISmsService
        TempData["Success"] = $"{sent} پیامک ارسال شد.";

        return RedirectToAction(nameof(SendAbsent), new { date, classId });
    }

    // ─── پیامک بدهی ───
    [HttpGet]
    public async Task<IActionResult> SendDebt(Guid? classId = null)
    {
        var result = await Mediator.Send(new PreviewDebtSmsQuery
        {
            ClassId = classId
        });

        var classes = await Mediator.Send(new GetClassesListQuery
        {
            ActiveOnly = true,
            PageSize = 500
        });

        ViewData["Title"] = "پیامک بدهی";

        return View(new DebtSmsPreviewViewModel
        {
            Data = result.IsSuccess ? result.Value! : new(),
            ClassId = classId,
            Classes = classes.Items.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name,
                Selected = classId == c.Id
            }).ToList()
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SendDebtSms(Guid? classId, List<Guid> studentIds)
    {
        if (studentIds is null || !studentIds.Any())
        {
            TempData["Error"] = "هیچ شاگردی انتخاب نشده است.";
            return RedirectToAction(nameof(SendDebt), new { classId });
        }

        var preview = await Mediator.Send(new PreviewDebtSmsQuery
        {
            ClassId = classId
        });

        if (!preview.IsSuccess)
        {
            TempData["Error"] = preview.Error;
            return RedirectToAction(nameof(SendDebt), new { classId });
        }

        var recipients = preview.Value!.Recipients
            .Where(r => studentIds.Contains(r.StudentId))
            .ToList();

        int sent = 0;
        foreach (var r in recipients)
        {
            var result = await Mediator.Send(new SendSmsCommand
            {
                Mobiles = new List<string> { r.Mobile },
                Message = r.RenderedMessage
            });

            if (result.IsSuccess && result.Value > 0)
                sent++;
        }

        TempData["Success"] = $"{sent} پیامک ارسال شد.";
        return RedirectToAction(nameof(SendDebt), new { classId });
    }



    [HttpGet]
    public async Task<IActionResult> History(
    string? search = null,
    SmsType? type = null,
    SmsStatus? status = null,
    DateTime? fromDate = null,
    DateTime? toDate = null,
    int page = 1)
    {
        ViewData["Title"] = "تاریخچه پیامک‌ها";

        var logs = await Mediator.Send(new GetSmsHistoryQuery
        {
            Search = search,
            Type = type,
            Status = status,
            FromDate = fromDate,
            ToDate = toDate,
            Page = page,
            PageSize = 30
        });

        var stats = await Mediator.Send(new GetSmsStatsQuery());

        return View(new SmsHistoryViewModel
        {
            Logs = logs,
            Stats = stats.IsSuccess ? stats.Value! : new(),
            Search = search,
            Type = type,
            Status = status,
            FromDate = fromDate,
            ToDate = toDate,
            TypeOptions = BuildSmsTypeOptions(type),
            StatusOptions = BuildSmsStatusOptions(status)
        });
    }

    private static List<SelectListItem> BuildSmsTypeOptions(SmsType? selected)
    {
        return new List<SelectListItem>
    {
        new() { Value = "", Text = "همه انواع", Selected = !selected.HasValue },
        new() { Value = ((int)SmsType.Absent).ToString(), Text = "غیبت",
                Selected = selected == SmsType.Absent },
        new() { Value = ((int)SmsType.Debt).ToString(), Text = "بدهی",
                Selected = selected == SmsType.Debt },
        new() { Value = ((int)SmsType.Announcement).ToString(), Text = "اطلاعیه",
                Selected = selected == SmsType.Announcement },
        new() { Value = ((int)SmsType.Manual).ToString(), Text = "دستی",
                Selected = selected == SmsType.Manual },
    };
    }

    private static List<SelectListItem> BuildSmsStatusOptions(SmsStatus? selected)
    {
        return new List<SelectListItem>
    {
        new() { Value = "", Text = "همه وضعیت‌ها", Selected = !selected.HasValue },
        new() { Value = ((int)SmsStatus.Sent).ToString(), Text = "ارسال‌شده",
                Selected = selected == SmsStatus.Sent },
        new() { Value = ((int)SmsStatus.Failed).ToString(), Text = "ناموفق",
                Selected = selected == SmsStatus.Failed },
        new() { Value = ((int)SmsStatus.Pending).ToString(), Text = "در انتظار",
                Selected = selected == SmsStatus.Pending },
    };
    }
}