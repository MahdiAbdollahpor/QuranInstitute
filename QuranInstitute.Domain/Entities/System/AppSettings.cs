using QuranInstitute.Domain.Common;

namespace QuranInstitute.Domain.Entities.System;

/// <summary>
/// تنظیمات موسسه — یک رکورد Singleton در دیتابیس
/// </summary>
public class AppSettings : BaseEntity
{
    public string InstituteName { get; set; } = "موسسه قرآنی";
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? LogoUrl { get; set; }

    /// <summary>متن قالب پیام غیبت</summary>
    public string AbsentTemplate { get; set; } =
        "{parent} گرامی، سلام. {student} در تاریخ {date} در کلاس «{class}» غایب بود. {institute}";

    /// <summary>متن قالب پیام بدهی</summary>
    public string DebtTemplate { get; set; } =
        "{parent} گرامی، سلام. مبلغ بدهی شهریه {student} تا امروز {amount} است. {institute}";

    /// <summary>متن قالب اطلاعیه عمومی</summary>
    public string GeneralTemplate { get; set; } = "";
}