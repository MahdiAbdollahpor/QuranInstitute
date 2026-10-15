using FluentValidation;

namespace QuranInstitute.Application.Features.Classes.Commands.CreateClass;

public class CreateClassCommandValidator : AbstractValidator<CreateClassCommand>
{
    public CreateClassCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("نام کلاس الزامی است.")
            .MaximumLength(150);

        RuleFor(x => x.TeacherName)
            .NotEmpty().WithMessage("نام استاد الزامی است.")
            .MaximumLength(150);

        RuleFor(x => x.ClassTime)
            .MaximumLength(100).When(x => !string.IsNullOrWhiteSpace(x.ClassTime));

        RuleFor(x => x.Capacity)
            .GreaterThan(0).WithMessage("ظرفیت باید بزرگ‌تر از صفر باشد.")
            .LessThanOrEqualTo(500).WithMessage("ظرفیت نمی‌تواند بیش از ۵۰۰ نفر باشد.");

        RuleFor(x => x.Days)
            .NotNull().WithMessage("روزهای برگزاری الزامی است.")
            .Must(d => d.Length > 0).WithMessage("حداقل یک روز هفته را انتخاب کنید.")
            .Must(d => d.All(x => x >= 0 && x <= 6))
            .WithMessage("روز هفته معتبر نیست.");

        RuleFor(x => x.DefaultOnlineLink)
            .MaximumLength(1000)
            .Must(BeValidUrl).When(x => !string.IsNullOrWhiteSpace(x.DefaultOnlineLink))
            .WithMessage("لینک کلاس آنلاین معتبر نیست.");
    }

    private static bool BeValidUrl(string? url)
    {
        return Uri.TryCreate(url, UriKind.Absolute, out var r)
               && (r.Scheme == Uri.UriSchemeHttp || r.Scheme == Uri.UriSchemeHttps);
    }
}