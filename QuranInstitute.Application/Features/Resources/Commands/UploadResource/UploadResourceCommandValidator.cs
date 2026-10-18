using FluentValidation;
using QuranInstitute.Domain.Enums;

namespace QuranInstitute.Application.Features.Resources.Commands.UploadResource;

public class UploadResourceCommandValidator : AbstractValidator<UploadResourceCommand>
{
    private static readonly string[] AllowedVideoTypes =
        { "video/mp4", "video/webm", "video/quicktime", "video/x-msvideo" };

    private static readonly string[] AllowedAudioTypes =
        { "audio/mpeg", "audio/mp4", "audio/ogg", "audio/wav", "audio/x-m4a" };

    public UploadResourceCommandValidator()
    {
        RuleFor(x => x.ClassId).NotEmpty();
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("عنوان منبع الزامی است.")
            .MaximumLength(200);

        RuleFor(x => x.Description).MaximumLength(2000);

        // اگر Type = Link، باید ExternalUrl باشد
        When(x => x.Type == ResourceType.Link, () =>
        {
            RuleFor(x => x.ExternalUrl)
                .NotEmpty().WithMessage("آدرس لینک الزامی است.")
                .Must(BeValidUrl).WithMessage("آدرس لینک معتبر نیست.");
        });

        // اگر Type != Link، باید فایل باشد
        When(x => x.Type != ResourceType.Link, () =>
        {
            RuleFor(x => x.FileStream)
                .NotNull().WithMessage("فایل را انتخاب کنید.");
            RuleFor(x => x.FileName)
                .NotEmpty().WithMessage("نام فایل الزامی است.");
        });

        RuleFor(x => x.FileSize)
            .LessThanOrEqualTo(500L * 1024 * 1024)
            .When(x => x.FileSize.HasValue)
            .WithMessage("حجم فایل نباید بیش از ۵۰۰ مگابایت باشد.");
    }

    private static bool BeValidUrl(string? url)
    {
        return Uri.TryCreate(url, UriKind.Absolute, out var r)
               && (r.Scheme == Uri.UriSchemeHttp || r.Scheme == Uri.UriSchemeHttps);
    }
}