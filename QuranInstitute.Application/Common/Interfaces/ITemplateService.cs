namespace QuranInstitute.Application.Common.Interfaces;

public interface ITemplateService
{
    /// <summary>
    /// جایگزینی متغیرها در قالب.
    /// متغیرها: {student}, {parent}, {class}, {date}, {amount}, {institute}
    /// </summary>
    string Render(string template, Dictionary<string, string> variables);

    /// <summary>گرفتن قالب از تنظیمات</summary>
    Task<string> GetTemplateAsync(string key, CancellationToken ct = default);
}