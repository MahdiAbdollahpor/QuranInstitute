using FluentValidation;

namespace QuranInstitute.Application.Features.Payments.Commands.RecordManualPayment;

public class RecordManualPaymentCommandValidator
    : AbstractValidator<RecordManualPaymentCommand>
{
    public RecordManualPaymentCommandValidator()
    {
        RuleFor(x => x.StudentId)
            .NotEmpty().WithMessage("شاگرد را انتخاب کنید.");

        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("مبلغ باید بزرگ‌تر از صفر باشد.");

        RuleFor(x => x.Date)
            .NotEmpty().WithMessage("تاریخ پرداخت الزامی است.")
            .LessThanOrEqualTo(DateTime.Today.AddDays(1))
            .WithMessage("تاریخ پرداخت نمی‌تواند در آینده باشد.");

        RuleFor(x => x.Method)
            .IsInEnum().WithMessage("روش پرداخت معتبر نیست.");

        RuleFor(x => x.Note)
            .MaximumLength(500).When(x => !string.IsNullOrWhiteSpace(x.Note));
    }
}