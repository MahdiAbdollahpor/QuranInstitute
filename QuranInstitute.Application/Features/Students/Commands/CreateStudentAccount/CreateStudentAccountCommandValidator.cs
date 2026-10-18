using FluentValidation;

namespace QuranInstitute.Application.Features.Students.Commands.CreateStudentAccount;

public class CreateStudentAccountCommandValidator
    : AbstractValidator<CreateStudentAccountCommand>
{
    public CreateStudentAccountCommandValidator()
    {
        RuleFor(x => x.StudentId).NotEmpty();

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("رمز عبور الزامی است.")
            .MinimumLength(6).WithMessage("رمز عبور حداقل ۶ کاراکتر باشد.");
    }
}