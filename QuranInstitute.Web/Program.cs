using Microsoft.AspNetCore.Identity;
using Parbad.Builder;
using Parbad.Gateway.ParbadVirtual;
using QuranInstitute.Application;
using QuranInstitute.Domain.Entities.Users;
using QuranInstitute.Infrastructure;
using QuranInstitute.Infrastructure.Persistence.Context;
using QuranInstitute.Infrastructure.Persistence.Seed;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);



builder.Services.AddParbad()
    .ConfigureGateways(gateways =>
    {
        // ★ درگاه مجازی برای تست
        gateways
            .AddParbadVirtual()
            .WithOptions(options =>
            {
                options.GatewayPath = "/MyVirtualGateway";   // مسیر درگاه مجازی
            });
    })
    .ConfigureHttpContext(builder => builder.UseDefaultAspNetCore())
    .ConfigureStorage(builder => builder.UseMemoryCache());


builder.Services.AddScoped<UserManager<AppUser>>();
builder.Services.AddScoped<RoleManager<AppRole>>();

// ─── Identity ────────────────────────────────────────
builder.Services
    .AddIdentity<AppUser, AppRole>(opt =>
    {
        opt.Password.RequiredLength = 6;
        opt.Password.RequireDigit = true;
        opt.Password.RequireUppercase = false;
        opt.Password.RequireLowercase = false;
        opt.Password.RequireNonAlphanumeric = false;

        opt.User.RequireUniqueEmail = false;
        opt.SignIn.RequireConfirmedPhoneNumber = false;
        opt.Lockout.MaxFailedAccessAttempts = 5;
        opt.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(opt =>
{
    opt.LoginPath = "/Account/Login";
    opt.LogoutPath = "/Account/Logout";
    opt.AccessDeniedPath = "/Account/AccessDenied";
    opt.ExpireTimeSpan = TimeSpan.FromDays(14);
    opt.SlidingExpiration = true;
    opt.Cookie.Name = "QuranInstitute.Auth";
    opt.Cookie.HttpOnly = true;
});

builder.Services.AddControllersWithViews();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    try
    {
        await DatabaseSeeder.SeedAsync(scope.ServiceProvider);
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "خطا در Seeder");
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseParbadVirtualGateway();

app.UseAuthentication();   // ★ مهم: قبل از Authorization
app.UseAuthorization();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();