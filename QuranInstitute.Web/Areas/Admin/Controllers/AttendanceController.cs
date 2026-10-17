using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using QuranInstitute.Application.Features.Attendances.Commands.BulkRecordAttendance;
using QuranInstitute.Application.Features.Attendances.Commands.MarkAllPresent;
using QuranInstitute.Application.Features.Attendances.Queries.GetAttendanceByClassDate;
using QuranInstitute.Application.Features.Attendances.Queries.GetTodayAbsentees;
using QuranInstitute.Application.Features.Classes.Queries.GetClassesList;
using QuranInstitute.Domain.Constants;
using QuranInstitute.Domain.Enums;
using QuranInstitute.Web.ViewModels.Attendances;

namespace QuranInstitute.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = $"{Roles.Admin},{Roles.Teacher}")]
public class AttendanceController : MediatorControllerBase
{
    public AttendanceController(IMediator mediator) : base(mediator) { }

    // ─────────────── INDEX (فرم حضور و غیاب) ───────────────
    [HttpGet]
    public async Task<IActionResult> Index(Guid? classId = null, string? date = null)
    {
        ViewData["Title"] = "حضور و غیاب";

        // انتخاب کلاس پیش‌فرض
        var classes = await Mediator.Send(new GetClassesListQuery
        {
            ActiveOnly = true,
            PageSize = 500
        });

        if (!classes.Items.Any())
        {
            TempData["Error"] = "ابتدا باید حداقل یک کلاس تعریف کنید.";
            return View(new AttendanceFormViewModel());
        }

        var selectedClassId = classId ?? classes.Items.First().Id;

        // تاریخ انتخابی
        var selectedDate = ParseDate(date);

        // دریافت داده فرم
        var result = await Mediator.Send(
            new GetAttendanceByClassDateQuery(selectedClassId, selectedDate));

        if (!result.IsSuccess)
        {
            TempData["Error"] = result.Error;
            return View(new AttendanceFormViewModel());
        }

        var data = result.Value!;

        var vm = new AttendanceFormViewModel
        {
            ClassId = data.ClassId,
            ClassName = data.ClassName,
            TeacherName = data.TeacherName,
            Days = data.Days,
            Date = data.Date,
            IsClassDay = data.IsClassDay,
            AlreadyRecorded = data.AlreadyRecorded,
            Entries = data.Entries,
            Classes = classes.Items.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name,
                Selected = c.Id == selectedClassId
            }).ToList()
        };

        return View(vm);
    }

    // ─────────────── SAVE (ثبت گروهی) ───────────────
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Save(
        Guid classId,
        DateTime date,
        [FromForm] List<Guid> studentIds,
        [FromForm] List<string> statuses,
        [FromForm] List<string> notes)
    {
        if (studentIds.Count != statuses.Count)
        {
            TempData["Error"] = "داده‌های فرم نامعتبر است.";
            return RedirectToAction(nameof(Index), new { classId, date = date.ToString("yyyy-MM-dd") });
        }

        var items = new List<AttendanceInput>();

        for (int i = 0; i < studentIds.Count; i++)
        {
            if (string.IsNullOrWhiteSpace(statuses[i]))
                continue;

            if (!Enum.TryParse<AttendanceStatus>(statuses[i], out var status))
                continue;

            items.Add(new AttendanceInput
            {
                StudentId = studentIds[i],
                Status = status,
                Note = i < notes.Count ? notes[i] : null
            });
        }

        if (!items.Any())
        {
            TempData["Error"] = "هیچ وضعیتی انتخاب نشده است.";
            return RedirectToAction(nameof(Index), new { classId, date = date.ToString("yyyy-MM-dd") });
        }

        var result = await Mediator.Send(new BulkRecordAttendanceCommand
        {
            ClassId = classId,
            Date = date,
            Items = items
        });

        TempData[result.IsSuccess ? "Success" : "Error"] =
            result.IsSuccess ? "حضور و غیاب با موفقیت ثبت شد." : result.Error;

        return RedirectToAction(nameof(Index), new
        {
            classId,
            date = date.ToString("yyyy-MM-dd")
        });
    }

    // ─────────────── MARK ALL PRESENT ───────────────
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MarkAllPresent(Guid classId, DateTime date)
    {
        var result = await Mediator.Send(new MarkAllPresentCommand(classId, date));

        TempData[result.IsSuccess ? "Success" : "Error"] =
            result.IsSuccess ? "همه شاگردان حاضر ثبت شدند." : result.Error;

        return RedirectToAction(nameof(Index), new
        {
            classId,
            date = date.ToString("yyyy-MM-dd")
        });
    }

    // ─────────────── HELPERS ───────────────
    private static DateTime ParseDate(string? input)
    {
        if (string.IsNullOrWhiteSpace(input)) return DateTime.Today;

        // پشتیبانی از فرمت میلادی yyyy-MM-dd
        if (DateTime.TryParse(input, out var dt)) return dt.Date;

        return DateTime.Today;
    }
}