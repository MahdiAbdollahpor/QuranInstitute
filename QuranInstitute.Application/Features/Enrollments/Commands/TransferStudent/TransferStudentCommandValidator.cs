using FluentValidation;

namespace QuranInstitute.Application.Features.Enrollments.Commands.TransferStudent;

public class TransferStudentCommandValidator : AbstractValidator<TransferStudentCommand>
{
    public TransferStudentCommandValidator()
    {
        RuleFor(x => x.StudentId).NotEmpty();
        RuleFor(x => x.FromClassId).NotEmpty();
        RuleFor(x => x.ToClassId).NotEmpty();

        RuleFor(x => x)
            .Must(x => x.FromClassId != x.ToClassId)
            .WithMessage("کلاس مبدأ و مقصد نمی‌توانند یکسان باشند.");
    }
}