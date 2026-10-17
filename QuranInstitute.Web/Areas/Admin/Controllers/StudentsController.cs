using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using QuranInstitute.Application.Common.Models;
using QuranInstitute.Application.Features.Students.Commands.CreateStudent;
using QuranInstitute.Application.Features.Students.Commands.CreateStudentAccount;
using QuranInstitute.Application.Features.Students.Commands.DeactivateStudent;
using QuranInstitute.Application.Features.Students.Commands.UpdateStudent;
using QuranInstitute.Application.Features.Students.Queries.GetClassesForSelect;
using QuranInstitute.Application.Features.Students.Queries.GetStudentById;
using QuranInstitute.Application.Features.Students.Queries.GetStudentsList;
using QuranInstitute.Domain.Constants;
using QuranInstitute.Web.ViewModels.Students;

namespace QuranInstitute.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = $"{Roles.Admin},{Roles.Teacher}")]
public class StudentsController : Controller
{
    private readonly IMediator _mediator;

    public StudentsController(IMediator mediator) => _mediator = mediator;

    // ─────────────── INDEX ───────────────
    [HttpGet]
    public async Task<IActionResult> Index(
        string? search = null, Guid? classId = null, int page = 1)
    {
        ViewData["Title"] = "شاگردان";

        var students = await _mediator.Send(new GetStudentsListQuery
        {
            Search = search,
            ClassId = classId,
            Page = page,
            PageSize = 20
        });

        var classes = await _mediator.Send(new GetClassesForSelectQuery());

        var vm = new StudentListViewModel
        {
            Students = students,
            Search = search,
            ClassId = classId,
            Classes = classes.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name,
                Selected = classId == c.Id
            }).ToList()
        };

        return View(vm);
    }

    // ─────────────── CREATE ───────────────
    [HttpGet]
    public async Task<IActionResult> Create()
    {
        ViewData["Title"] = "ثبت شاگرد جدید";

        var vm = new CreateStudentViewModel
        {
            Classes = await GetClassSelectList()
        };

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateStudentViewModel vm)
    {
        ViewData["Title"] = "ثبت شاگرد جدید";

        if (!ModelState.IsValid)
        {
            vm.Classes = await GetClassSelectList(vm.ClassId);
            return View(vm);
        }

        var result = await _mediator.Send(new CreateStudentCommand
        {
            Name = vm.Name,
            ParentName = vm.ParentName,
            ParentMobile = vm.ParentMobile,
            ClassId = vm.ClassId,
            MonthlyFee = vm.MonthlyFee,
            StartDate = vm.StartDate,
            Note = vm.Note
        });

        if (!result.IsSuccess)
        {
            ModelState.AddModelError("", result.Error!);
            vm.Classes = await GetClassSelectList(vm.ClassId);
            return View(vm);
        }

        TempData["Success"] = "شاگرد با موفقیت ثبت شد.";
        return RedirectToAction(nameof(Index));
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateAccount(
    Guid studentId, string password, string role)
    {
        var result = await _mediator.Send(
            new CreateStudentAccountCommand(studentId, password, role));

        TempData[result.IsSuccess ? "Success" : "Error"] = result.IsSuccess
            ? "حساب کاربری ساخته شد. کاربر می‌تواند با موبایل والد و رمز وارد شود."
            : result.Error;

        return RedirectToAction(nameof(Index));
    }

    // ─────────────── EDIT ───────────────
    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        ViewData["Title"] = "ویرایش شاگرد";

        var result = await _mediator.Send(new GetStudentByIdQuery(id));
        if (!result.IsSuccess)
        {
            TempData["Error"] = result.Error;
            return RedirectToAction(nameof(Index));
        }

        var s = result.Value!;
        var vm = new EditStudentViewModel
        {
            Id = s.Id,
            Name = s.Name,
            ParentName = s.ParentName,
            ParentMobile = s.ParentMobile,
            ClassId = s.ClassId,
            MonthlyFee = s.MonthlyFee,
            StartDate = s.StartDate,
            Note = s.Note,
            Classes = await GetClassSelectList(s.ClassId)
        };

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(EditStudentViewModel vm)
    {
        ViewData["Title"] = "ویرایش شاگرد";

        if (!ModelState.IsValid)
        {
            vm.Classes = await GetClassSelectList(vm.ClassId);
            return View(vm);
        }

        var result = await _mediator.Send(new UpdateStudentCommand
        {
            Id = vm.Id,
            Name = vm.Name,
            ParentName = vm.ParentName,
            ParentMobile = vm.ParentMobile,
            ClassId = vm.ClassId,
            MonthlyFee = vm.MonthlyFee,
            StartDate = vm.StartDate,
            Note = vm.Note
        });

        if (!result.IsSuccess)
        {
            ModelState.AddModelError("", result.Error!);
            vm.Classes = await GetClassSelectList(vm.ClassId);
            return View(vm);
        }

        TempData["Success"] = "تغییرات ذخیره شد.";
        return RedirectToAction(nameof(Index));
    }

    // ─────────────── DEACTIVATE ───────────────
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Deactivate(Guid id)
    {
        var result = await _mediator.Send(new DeactivateStudentCommand(id));

        TempData[result.IsSuccess ? "Success" : "Error"] =
            result.IsSuccess ? "شاگرد غیرفعال شد." : result.Error;

        return RedirectToAction(nameof(Index));
    }

    // ─────────────── HELPERS ───────────────
    private async Task<List<SelectListItem>> GetClassSelectList(Guid? selected = null)
    {
        var classes = await _mediator.Send(new GetClassesForSelectQuery());

        return classes.Select(c => new SelectListItem
        {
            Value = c.Id.ToString(),
            Text = c.Name,
            Selected = selected.HasValue && selected.Value == c.Id
        }).ToList();
    }
}