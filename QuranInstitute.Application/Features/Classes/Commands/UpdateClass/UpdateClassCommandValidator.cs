using FluentValidation;

namespace QuranInstitute.Application.Features.Classes.Commands.UpdateClass;

public class UpdateClassCommandValidator : AbstractValidator<UpdateClassCommand>
{
    public UpdateClassCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("نام کلاس الزامی است.")
            .MaximumLength(150);

        RuleFor(x => x.TeacherName)
            .NotEmpty().WithMessage("نام استاد الزامی است.")
            .MaximumLength(150);

        RuleFor(x => x.Capacity)
            .GreaterThan(0)
            .LessThanOrEqualTo(500);

        RuleFor(x => x.Days)
            .NotNull()
            .Must(d => d.Length > 0).WithMessage("حداقل یک روز هفته را انتخاب کنید.")
            .Must(d => d.All(x => x >= 0 && x <= 6));

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