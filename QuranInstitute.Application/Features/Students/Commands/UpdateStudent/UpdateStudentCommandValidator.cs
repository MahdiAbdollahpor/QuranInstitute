using FluentValidation;
using QuranInstitute.Shared.Helpers;

namespace QuranInstitute.Application.Features.Students.Commands.UpdateStudent;

public class UpdateStudentCommandValidator : AbstractValidator<UpdateStudentCommand>
{
    public UpdateStudentCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("نام شاگرد الزامی است.")
            .MaximumLength(150);

        RuleFor(x => x.ParentName)
            .NotEmpty().WithMessage("نام ولی الزامی است.")
            .MaximumLength(150);

        RuleFor(x => x.ParentMobile)
            .NotEmpty().WithMessage("شماره موبایل ولی الزامی است.")
            .Must(BeValidMobile).WithMessage("شماره موبایل معتبر نیست.");

        RuleFor(x => x.MonthlyFee).GreaterThanOrEqualTo(0);
    }

    private static bool BeValidMobile(string? mobile)
    {
        if (string.IsNullOrWhiteSpace(mobile)) return false;
        var n = mobile.ConvertToEn().Trim().Replace(" ", "").Replace("-", "");
        if (n.StartsWith("+98")) n = "0" + n.Substring(3);
        if (n.StartsWith("0098")) n = "0" + n.Substring(4);
        return System.Text.RegularExpressions.Regex.IsMatch(n, @"^09\d{9}$");
    }
}