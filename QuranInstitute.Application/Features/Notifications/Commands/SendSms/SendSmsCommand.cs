using MediatR;
using QuranInstitute.Application.Common.Models;

namespace QuranInstitute.Application.Features.Notifications.Commands.SendSms;

public class SendSmsCommand : IRequest<Result<int>>
{
    public List<string> Mobiles { get; set; } = new();
    public string Message { get; set; } = string.Empty;
}