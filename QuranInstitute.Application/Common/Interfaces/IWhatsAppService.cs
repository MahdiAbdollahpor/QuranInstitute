namespace QuranInstitute.Application.Common.Interfaces;

public interface IWhatsAppService
{
    /// <summary>ساخت لینک wa.me با پیام آماده</summary>
    string BuildLink(string mobile, string message);
}