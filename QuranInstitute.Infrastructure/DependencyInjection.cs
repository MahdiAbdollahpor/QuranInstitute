using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using QuranInstitute.Application.Common.Interfaces;
using QuranInstitute.Application.Features.Tuition.Services;
using QuranInstitute.Infrastructure.Persistence.Context;
using QuranInstitute.Infrastructure.Services;
using QuranInstitute.Infrastructure.Services.Backup;
using QuranInstitute.Infrastructure.Services.FileStorage;
using QuranInstitute.Infrastructure.Services.Sms;
using QuranInstitute.Infrastructure.Services.Tuition;
using QuranInstitute.Infrastructure.Services.WhatsApp;

namespace QuranInstitute.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var cs = configuration.GetConnectionString("DefaultConnection")
                 ?? throw new InvalidOperationException("Connection string not found.");

        services.AddDbContext<ApplicationDbContext>(opt =>
            opt.UseSqlServer(cs, sql =>
            {
                sql.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);
                sql.EnableRetryOnFailure(3);
            }));

        services.AddScoped<IApplicationDbContext>(sp =>
            sp.GetRequiredService<ApplicationDbContext>());


        services.Configure<RemoteFileStorageOptions>(
    configuration.GetSection("FileStorage:Remote"));

        services.AddHttpClient<RemoteFileStorageService>();
        services.AddScoped<LocalFileStorageService>();

        services.AddScoped<IFileStorageService>(sp =>
        {
            var active = configuration["FileStorage:Active"] ?? "Local";

            return active == "Remote"
                ? sp.GetRequiredService<RemoteFileStorageService>()
                : sp.GetRequiredService<LocalFileStorageService>();
        });



        // ─── SMS ───
        services.Configure<KavenegarOptions>(configuration.GetSection("Sms:Kavenegar"));
        services.AddHttpClient<KavenegarSmsService>();
        services.AddScoped<NullSmsService>();

        // سرویس خام (بدون لاگ) — بر اساس config
        services.AddScoped<IRawSmsService>(sp =>
        {
            var active = configuration["Sms:Active"] ?? "Null";
            return active == "Kavenegar"
                ? sp.GetRequiredService<KavenegarSmsService>()
                : sp.GetRequiredService<NullSmsService>();
        });

        // سرویس اصلی با لاگ — در همه Commandها از این استفاده کن
        services.AddScoped<ISmsService, SmsServiceWithLogging>();



        services.AddHttpContextAccessor();

        services.AddScoped<IBackupService, BackupService>();
        services.AddScoped<ITuitionCalculator, TuitionCalculator>();
        services.AddScoped<IDateTimeService, DateTimeService>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<IFileStorageService, LocalFileStorageService>();
        services.AddScoped<IWhatsAppService, WhatsAppLinkBuilder>();
        services.AddScoped<IIdentityService, IdentityService>();
        services.AddScoped<ITemplateService, TemplateService>();

        return services;
    }
}