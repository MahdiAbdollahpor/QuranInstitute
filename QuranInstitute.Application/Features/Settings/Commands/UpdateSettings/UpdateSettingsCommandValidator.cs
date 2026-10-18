using FluentValidation;

namespace QuranInstitute.Application.Features.Settings.Commands.UpdateSettings;

public class UpdateSettingsCommandValidator : AbstractValidator<UpdateSettingsCommand>
{
    public UpdateSettingsCommandValidator()
    {
        RuleFor(x => x.InstituteName)
            .NotEmpty().WithMessage("نام موسسه الزامی است.")
            .MaximumLength(200);

        RuleFor(x => x.Address).MaximumLength(500);
        RuleFor(x => x.Phone).MaximumLength(20);
        RuleFor(x => x.Email).MaximumLength(150).EmailAddress()
            .When(x => !string.IsNullOrWhiteSpace(x.Email))
            .WithMessage("ایمیل معتبر نیست.");
        RuleFor(x => x.LogoUrl).MaximumLength(500);

        RuleFor(x => x.AbsentTemplate).MaximumLength(2000);
        RuleFor(x => x.DebtTemplate).MaximumLength(2000);
        RuleFor(x => x.GeneralTemplate).MaximumLength(2000);
    }
}