using MediatR;
using QuranInstitute.Application.Common.Interfaces;
using QuranInstitute.Application.Common.Models;
using QuranInstitute.Domain.Enums;

namespace QuranInstitute.Application.Features.Notifications.Commands.SendSms;

public class SendSmsCommandHandler : IRequestHandler<SendSmsCommand, Result<int>>
{
    private readonly ISmsService _sms;

    public SendSmsCommandHandler(ISmsService sms) => _sms = sms;

    public async Task<Result<int>> Handle(
        SendSmsCommand request, CancellationToken ct)
    {
        if (!request.Mobiles.Any())
            return Result<int>.Failure("گیرنده‌ای برای ارسال وجود ندارد.");

        if (string.IsNullOrWhiteSpace(request.Message))
            return Result<int>.Failure("متن پیام خالی است.");

        var requests = request.Mobiles.Select(m => new SmsSendRequest
        {
            Mobile = m,
            Message = request.Message,
            Type = SmsType.Manual
        }).ToList();

        var sent = await _sms.SendBulkAsync(requests, ct);
        return Result<int>.Success(sent);
    }
}