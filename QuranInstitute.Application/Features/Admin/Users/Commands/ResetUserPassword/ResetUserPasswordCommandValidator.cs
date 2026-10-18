using FluentValidation;

namespace QuranInstitute.Application.Features.Admin.Users.Commands.ResetUserPassword;

public class ResetUserPasswordCommandValidator
    : AbstractValidator<ResetUserPasswordCommand>
{
    public ResetUserPasswordCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage("رمز جدید الزامی است.")
            .MinimumLength(6).WithMessage("رمز جدید حداقل ۶ کاراکتر باشد.");
    }
}