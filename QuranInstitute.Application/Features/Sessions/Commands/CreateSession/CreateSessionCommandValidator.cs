using FluentValidation;

namespace QuranInstitute.Application.Features.Sessions.Commands.CreateSession;

public class CreateSessionCommandValidator : AbstractValidator<CreateSessionCommand>
{
    public CreateSessionCommandValidator()
    {
        RuleFor(x => x.ClassId).NotEmpty();
        RuleFor(x => x.SessionNumber)
            .GreaterThan(0).WithMessage("شماره جلسه باید بزرگ‌تر از صفر باشد.");
        RuleFor(x => x.Date).NotEmpty();
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("عنوان جلسه الزامی است.")
            .MaximumLength(200);
        RuleFor(x => x.Description).MaximumLength(2000);
        RuleFor(x => x.OnlineLink).MaximumLength(1000);
        RuleFor(x => x.RecordingUrl).MaximumLength(1000);
    }
}