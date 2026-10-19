using FluentValidation;

namespace QuranInstitute.Application.Features.Notifications.Commands.CreateAnnouncement;

public class CreateAnnouncementCommandValidator
    : AbstractValidator<CreateAnnouncementCommand>
{
    public CreateAnnouncementCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("عنوان الزامی است.")
            .MaximumLength(200);

        RuleFor(x => x.Body)
            .NotEmpty().WithMessage("متن اطلاعیه الزامی است.")
            .MaximumLength(4000);

        RuleFor(x => x)
            .Must(x => x.SendSms || x.SendWhatsApp || x.SendInApp)
            .WithMessage("حداقل یک کانال ارسال را انتخاب کنید.");

        RuleFor(x => x)
            .Must(x => !(x.TargetClassId.HasValue && x.TargetStudentId.HasValue))
            .WithMessage("نمی‌توانید همزمان کلاس و شاگرد را انتخاب کنید.");
    }
}