using FluentValidation;
using QuranInstitute.Shared.Helpers;

namespace QuranInstitute.Application.Features.Students.Commands.CreateStudent;

public class CreateStudentCommandValidator : AbstractValidator<CreateStudentCommand>
{
    public CreateStudentCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("نام شاگرد الزامی است.")
            .MaximumLength(150);

        RuleFor(x => x.ParentName)
            .NotEmpty().WithMessage("نام ولی الزامی است.")
            .MaximumLength(150);

        RuleFor(x => x.ParentMobile)
            .NotEmpty().WithMessage("شماره موبایل ولی الزامی است.")
            .Must(BeValidMobile).WithMessage("شماره موبایل معتبر نیست (مثال: 09123456789).");

        RuleFor(x => x.MonthlyFee)
            .GreaterThanOrEqualTo(0).WithMessage("شهریه نمی‌تواند منفی باشد.");

        RuleFor(x => x.StartDate)
            .NotEmpty().WithMessage("تاریخ شروع الزامی است.");
    }

    private static bool BeValidMobile(string? mobile)
    {
        if (string.IsNullOrWhiteSpace(mobile)) return false;

        var normalized = mobile.ConvertToEn().Trim().Replace(" ", "").Replace("-", "");
        if (normalized.StartsWith("+98")) normalized = "0" + normalized.Substring(3);
        if (normalized.StartsWith("0098")) normalized = "0" + normalized.Substring(4);
        if (normalized.StartsWith("98") && normalized.Length == 12)
            normalized = "0" + normalized.Substring(2);

        return System.Text.RegularExpressions.Regex.IsMatch(normalized, @"^09\d{9}$");
    }
}