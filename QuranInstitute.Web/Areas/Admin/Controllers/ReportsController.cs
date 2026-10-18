using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using QuranInstitute.Application.Features.Classes.Queries.GetClassesList;
using QuranInstitute.Application.Features.Reports.Queries.ExportData;
using QuranInstitute.Application.Features.Reports.Queries.GetFinancialSummary;
using QuranInstitute.Application.Features.Reports.Queries.GetMonthlyAttendanceReport;
using QuranInstitute.Domain.Constants;
using QuranInstitute.Shared.Helpers;
using QuranInstitute.Web.ViewModels.Reports;
using System.Text;

namespace QuranInstitute.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = $"{Roles.Admin},{Roles.Teacher}")]
public class ReportsController : MediatorControllerBase
{
    private static readonly string[] MonthNames =
    {
        "فروردین", "اردیبهشت", "خرداد", "تیر", "مرداد", "شهریور",
        "مهر", "آبان", "آذر", "دی", "بهمن", "اسفند"
    };

    public ReportsController(IMediator mediator) : base(mediator) { }

    // ─── صفحه اصلی گزارش‌ها ───
    [HttpGet]
    public IActionResult Index()
    {
        var today = DateTime.Today;
        var j = MyDateTime.GetYearOfGregorianDateInShamsi(today);
        var m = MyDateTime.GetMonthOfGregorianDateInShamsi(today);

        ViewData["Title"] = "گزارش‌ها";
        return View(new ReportsIndexViewModel
        {
            CurrentJalaliYear = j,
            CurrentJalaliMonth = m,
            CurrentMonthName = MonthNames[m - 1]
        });
    }

    // ─── گزارش حضور ماهانه ───
    [HttpGet]
    public async Task<IActionResult> Attendance(
        int? year = null, int? month = null, Guid? classId = null)
    {
        var today = DateTime.Today;
        var j = year ?? MyDateTime.GetYearOfGregorianDateInShamsi(today);
        var m = month ?? MyDateTime.GetMonthOfGregorianDateInShamsi(today);

        var result = await Mediator.Send(new GetMonthlyAttendanceReportQuery
        {
            JalaliYear = j,
            JalaliMonth = m,
            ClassId = classId
        });

        var classes = await Mediator.Send(new GetClassesListQuery
        {
            ActiveOnly = true,
            PageSize = 500
        });

        ViewData["Title"] = $"گزارش حضور {MonthNames[m - 1]} {j}";

        return View(new AttendanceReportViewModel
        {
            Data = result.IsSuccess ? result.Value! : new(),
            SelectedYear = j,
            SelectedMonth = m,
            ClassId = classId,
            Classes = classes.Items.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name,
                Selected = classId == c.Id
            }).ToList()
        });
    }

    // ─── گزارش مالی ماهانه ───
    [HttpGet]
    public async Task<IActionResult> Financial(int? year = null, int? month = null)
    {
        var today = DateTime.Today;
        var j = year ?? MyDateTime.GetYearOfGregorianDateInShamsi(today);
        var m = month ?? MyDateTime.GetMonthOfGregorianDateInShamsi(today);

        var result = await Mediator.Send(new GetFinancialSummaryQuery
        {
            JalaliYear = j,
            JalaliMonth = m
        });

        ViewData["Title"] = $"گزارش مالی {MonthNames[m - 1]} {j}";

        return View(new FinancialReportViewModel
        {
            Data = result.IsSuccess ? result.Value! : new(),
            SelectedYear = j,
            SelectedMonth = m
        });
    }

    // ─── خروجی CSV — شاگردان ───
    [HttpGet]
    public async Task<IActionResult> ExportStudentsCsv()
    {
        var csv = await Mediator.Send(new ExportStudentsCsvQuery { ActiveOnly = true });
        var bytes = Encoding.UTF8.GetPreamble()
            .Concat(Encoding.UTF8.GetBytes(csv)).ToArray();

        return File(bytes, "text/csv; charset=utf-8",
            $"students-{DateTime.Now:yyyyMMdd}.csv");
    }

    // ─── خروجی CSV — پرداخت‌ها ───
    [HttpGet]
    public async Task<IActionResult> ExportPaymentsCsv(DateTime? from, DateTime? to)
    {
        var csv = await Mediator.Send(new ExportPaymentsCsvQuery
        {
            FromDate = from,
            ToDate = to
        });

        var bytes = Encoding.UTF8.GetPreamble()
            .Concat(Encoding.UTF8.GetBytes(csv)).ToArray();

        return File(bytes, "text/csv; charset=utf-8",
            $"payments-{DateTime.Now:yyyyMMdd}.csv");
    }

    // ─── خروجی CSV — حضور و غیاب ───
    [HttpGet]
    public async Task<IActionResult> ExportAttendanceCsv(int? year, int? month)
    {
        var today = DateTime.Today;
        var j = year ?? MyDateTime.GetYearOfGregorianDateInShamsi(today);
        var m = month ?? MyDateTime.GetMonthOfGregorianDateInShamsi(today);

        var csv = await Mediator.Send(new ExportAttendanceCsvQuery
        {
            JalaliYear = j,
            JalaliMonth = m
        });

        var bytes = Encoding.UTF8.GetPreamble()
            .Concat(Encoding.UTF8.GetBytes(csv)).ToArray();

        return File(bytes, "text/csv; charset=utf-8",
            $"attendance-{j}-{m:00}.csv");
    }
}