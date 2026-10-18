using FluentValidation;

namespace QuranInstitute.Application.Features.UserPanel.Commands.ChangeMyPassword;

public class ChangeMyPasswordCommandValidator
    : AbstractValidator<ChangeMyPasswordCommand>
{
    public ChangeMyPasswordCommandValidator()
    {
        RuleFor(x => x.CurrentPassword)
            .NotEmpty().WithMessage("رمز فعلی الزامی است.");

        RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage("رمز جدید الزامی است.")
            .MinimumLength(6).WithMessage("رمز جدید حداقل ۶ کاراکتر باشد.")
            .NotEqual(x => x.CurrentPassword)
            .WithMessage("رمز جدید باید با رمز فعلی متفاوت باشد.");

        RuleFor(x => x.ConfirmPassword)
            .Equal(x => x.NewPassword)
            .WithMessage("تکرار رمز جدید مطابقت ندارد.");
    }
}