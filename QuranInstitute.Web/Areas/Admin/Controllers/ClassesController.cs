using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using QuranInstitute.Application.Features.Classes.Commands.CreateClass;
using QuranInstitute.Application.Features.Classes.Commands.DeactivateClass;
using QuranInstitute.Application.Features.Classes.Commands.UpdateClass;
using QuranInstitute.Application.Features.Classes.Queries.GetClassById;
using QuranInstitute.Application.Features.Classes.Queries.GetClassesList;
using QuranInstitute.Domain.Constants;
using QuranInstitute.Domain.Enums;
using QuranInstitute.Web.ViewModels.Classes;

namespace QuranInstitute.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = $"{Roles.Admin},{Roles.Teacher}")]
public class ClassesController : MediatorControllerBase
{
    public ClassesController(IMediator mediator) : base(mediator) { }

    // ─────────────── INDEX ───────────────
    [HttpGet]
    public async Task<IActionResult> Index(string? search = null, int page = 1)
    {
        ViewData["Title"] = "کلاس‌ها";

        var result = await Mediator.Send(new GetClassesListQuery
        {
            Search = search,
            Page = page,
            PageSize = 20
        });

        var vm = new ClassListViewModel
        {
            Classes = result,
            Search = search
        };

        return View(vm);
    }

    // ─────────────── CREATE ───────────────
    [HttpGet]
    public IActionResult Create()
    {
        ViewData["Title"] = "تعریف کلاس جدید";

        var vm = new CreateClassViewModel
        {
            DayOptions = BuildDayOptions(),
            TypeOptions = BuildTypeOptions()
        };

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateClassViewModel vm)
    {
        ViewData["Title"] = "تعریف کلاس جدید";

        // اعتبارسنجی: حداقل یک روز
        if (vm.SelectedDays is null || vm.SelectedDays.Count == 0)
            ModelState.AddModelError(nameof(vm.SelectedDays), "حداقل یک روز هفته را انتخاب کنید.");

        if (!ModelState.IsValid)
        {
            vm.DayOptions = BuildDayOptions(vm.SelectedDays);
            vm.TypeOptions = BuildTypeOptions(vm.Type);
            return View(vm);
        }

        var result = await Mediator.Send(new CreateClassCommand
        {
            Name = vm.Name,
            TeacherName = vm.TeacherName,
            ClassTime = vm.ClassTime,
            Days = vm.SelectedDays.ToArray(),
            Type = vm.Type,
            DefaultOnlineLink = vm.DefaultOnlineLink,
            Capacity = vm.Capacity
        });

        if (!result.IsSuccess)
        {
            ModelState.AddModelError("", result.Error!);
            vm.DayOptions = BuildDayOptions(vm.SelectedDays);
            vm.TypeOptions = BuildTypeOptions(vm.Type);
            return View(vm);
        }

        TempData["Success"] = "کلاس با موفقیت ثبت شد.";
        return RedirectToAction(nameof(Index));
    }

    // ─────────────── EDIT ───────────────
    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        ViewData["Title"] = "ویرایش کلاس";

        var result = await Mediator.Send(new GetClassByIdQuery(id));
        if (!result.IsSuccess)
        {
            TempData["Error"] = result.Error;
            return RedirectToAction(nameof(Index));
        }

        var c = result.Value!;
        var vm = new EditClassViewModel
        {
            Id = c.Id,
            Name = c.Name,
            TeacherName = c.TeacherName,
            ClassTime = c.ClassTime,
            SelectedDays = c.Days.ToList(),
            Type = c.Type,
            DefaultOnlineLink = c.DefaultOnlineLink,
            Capacity = c.Capacity,
            DayOptions = BuildDayOptions(c.Days.ToList()),
            TypeOptions = BuildTypeOptions(c.Type)
        };

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(EditClassViewModel vm)
    {
        ViewData["Title"] = "ویرایش کلاس";

        if (vm.SelectedDays is null || vm.SelectedDays.Count == 0)
            ModelState.AddModelError(nameof(vm.SelectedDays), "حداقل یک روز هفته را انتخاب کنید.");

        if (!ModelState.IsValid)
        {
            vm.DayOptions = BuildDayOptions(vm.SelectedDays);
            vm.TypeOptions = BuildTypeOptions(vm.Type);
            return View(vm);
        }

        var result = await Mediator.Send(new UpdateClassCommand
        {
            Id = vm.Id,
            Name = vm.Name,
            TeacherName = vm.TeacherName,
            ClassTime = vm.ClassTime,
            Days = vm.SelectedDays.ToArray(),
            Type = vm.Type,
            DefaultOnlineLink = vm.DefaultOnlineLink,
            Capacity = vm.Capacity
        });

        if (!result.IsSuccess)
        {
            ModelState.AddModelError("", result.Error!);
            vm.DayOptions = BuildDayOptions(vm.SelectedDays);
            vm.TypeOptions = BuildTypeOptions(vm.Type);
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
        var result = await Mediator.Send(new DeactivateClassCommand(id));

        TempData[result.IsSuccess ? "Success" : "Error"] =
            result.IsSuccess ? "کلاس غیرفعال شد." : result.Error;

        return RedirectToAction(nameof(Index));
    }

    // ─────────────── HELPERS ───────────────
    private static List<SelectListItem> BuildDayOptions(List<int>? selected = null)
    {
        var days = new[]
        {
            (Value: 6, Label: "شنبه"),
            (Value: 0, Label: "یکشنبه"),
            (Value: 1, Label: "دوشنبه"),
            (Value: 2, Label: "سه‌شنبه"),
            (Value: 3, Label: "چهارشنبه"),
            (Value: 4, Label: "پنجشنبه"),
            (Value: 5, Label: "جمعه"),
        };

        selected ??= new List<int>();

        return days.Select(d => new SelectListItem
        {
            Value = d.Value.ToString(),
            Text = d.Label,
            Selected = selected.Contains(d.Value)
        }).ToList();
    }

    private static List<SelectListItem> BuildTypeOptions(ClassType selected = ClassType.InPerson)
    {
        return new List<SelectListItem>
        {
            new() { Value = ((int)ClassType.InPerson).ToString(), Text = "حضوری",
                    Selected = selected == ClassType.InPerson },
            new() { Value = ((int)ClassType.Online).ToString(), Text = "مجازی",
                    Selected = selected == ClassType.Online },
            new() { Value = ((int)ClassType.Hybrid).ToString(), Text = "ترکیبی",
                    Selected = selected == ClassType.Hybrid },
        };
    }
}