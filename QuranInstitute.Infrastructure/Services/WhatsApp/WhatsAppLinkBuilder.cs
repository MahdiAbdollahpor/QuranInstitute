using QuranInstitute.Application.Common.Interfaces;
using QuranInstitute.Shared.Helpers;

namespace QuranInstitute.Infrastructure.Services.WhatsApp;

public class WhatsAppLinkBuilder : IWhatsAppService
{
    public string BuildLink(string mobile, string message)
    {
        var p = mobile.ConvertToEn().Replace("+", "").Replace(" ", "").Replace("-", "");

        if (p.StartsWith("00")) p = p.Substring(2);
        if (p.StartsWith("0")) p = "98" + p.Substring(1);

        var encoded = Uri.EscapeDataString(message);
        return $"https://wa.me/{p}?text={encoded}";
    }
}