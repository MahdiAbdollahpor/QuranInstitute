using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using QuranInstitute.Application.Features.Classes.Queries.GetClassesList;
using QuranInstitute.Application.Features.Tuition.Queries.GetDebtorsList;
using QuranInstitute.Application.Features.Tuition.Queries.GetStudentBalance;
using QuranInstitute.Domain.Constants;
using QuranInstitute.Web.ViewModels.Tuition;

namespace QuranInstitute.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = $"{Roles.Admin},{Roles.Teacher}")]
public class TuitionController : MediatorControllerBase
{
    public TuitionController(IMediator mediator) : base(mediator) { }

    // ─────────────── INDEX (لیست بدهکاران) ───────────────
    [HttpGet]
    public async Task<IActionResult> Index(
        string? search = null, Guid? classId = null, int page = 1)
    {
        ViewData["Title"] = "بدهکاران شهریه";

        var result = await Mediator.Send(new GetDebtorsListQuery
        {
            Search = search,
            ClassId = classId,
            Page = page,
            PageSize = 50
        });

        var classes = await Mediator.Send(new GetClassesListQuery
        {
            ActiveOnly = true,
            PageSize = 500
        });

        var vm = new TuitionListViewModel
        {
            Debtors = result,
            Search = search,
            ClassId = classId,
            Classes = classes.Items.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name,
                Selected = classId == c.Id
            }).ToList()
        };

        return View(vm);
    }

    // ─────────────── STUDENT BALANCE ───────────────
    [HttpGet]
    public async Task<IActionResult> Student(Guid id)
    {
        var result = await Mediator.Send(new GetStudentBalanceQuery(id));

        if (!result.IsSuccess)
        {
            TempData["Error"] = result.Error;
            return RedirectToAction(nameof(Index));
        }

        ViewData["Title"] = $"وضعیت حساب {result.Value!.StudentName}";
        return View(result.Value!);
    }
}