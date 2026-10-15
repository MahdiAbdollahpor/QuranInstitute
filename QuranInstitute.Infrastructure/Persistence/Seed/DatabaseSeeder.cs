using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using QuranInstitute.Domain.Constants;
using QuranInstitute.Domain.Entities.Communication;
using QuranInstitute.Domain.Entities.Users;
using QuranInstitute.Infrastructure.Persistence.Context;

namespace QuranInstitute.Infrastructure.Persistence.Seed;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(IServiceProvider sp)
    {
        var db = sp.GetRequiredService<ApplicationDbContext>();
        var userMgr = sp.GetRequiredService<UserManager<AppUser>>();
        var roleMgr = sp.GetRequiredService<RoleManager<AppRole>>();
        var logger = sp.GetRequiredService<ILoggerFactory>().CreateLogger("Seeder");

        await db.Database.MigrateAsync();

        // ─── ۱. نقش‌ها ────────────────────────────────
        foreach (var roleName in Roles.All)
        {
            if (!await roleMgr.RoleExistsAsync(roleName))
            {
                await roleMgr.CreateAsync(new AppRole(roleName)
                {
                    Description = roleName switch
                    {
                        Roles.Admin => "مدیر موسسه",
                        Roles.Teacher => "استاد",
                        Roles.Student => "شاگرد",
                        Roles.Parent => "والد",
                        _ => roleName
                    }
                });
                logger.LogInformation("Role created: {Role}", roleName);
            }
        }

        // ─── ۲. کاربر ادمین پیش‌فرض ──────────────────
        const string adminMobile = "09120000000";
        var admin = await userMgr.FindByNameAsync(adminMobile);

        if (admin is null)
        {
            admin = new AppUser
            {
                UserName = adminMobile,
                PhoneNumber = adminMobile,
                FullName = "مدیر موسسه",
                Email = "admin@quran.local",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                IsActive = true
            };

            var result = await userMgr.CreateAsync(admin, "Admin@123");
            if (result.Succeeded)
            {
                await userMgr.AddToRoleAsync(admin, Roles.Admin);
                logger.LogWarning("⚠ ادمین پیش‌فرض ساخته شد: موبایل={Mobile} رمز=Admin@123 — لطفاً تغییر دهید!",
                    adminMobile);
            }
            else
            {
                logger.LogError("خطا در ساخت ادمین: {Errors}",
                    string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }

        // ─── ۳. قالب‌های پیام پیش‌فرض ────────────────
        if (!await db.MessageTemplates.AnyAsync())
        {
            db.MessageTemplates.AddRange(
                new MessageTemplate
                {
                    Key = "absent",
                    Title = "اطلاعیه غیبت",
                    Body = "{parent} گرامی، سلام. {student} در تاریخ {date} در کلاس «{class}» غایب بود. لطفاً علت غیبت را اطلاع دهید. {institute}"
                },
                new MessageTemplate
                {
                    Key = "debt",
                    Title = "یادآوری شهریه",
                    Body = "{parent} گرامی، سلام. مبلغ بدهی شهریه {student} تا امروز {amount} است. خواهشمند است نسبت به پرداخت اقدام فرمایید. {institute}"
                },
                new MessageTemplate
                {
                    Key = "general",
                    Title = "اطلاعیه عمومی",
                    Body = ""
                }
            );

            await db.SaveChangesAsync();
            logger.LogInformation("Message templates seeded.");
        }
    }
}