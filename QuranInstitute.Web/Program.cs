using Microsoft.AspNetCore.Identity;
using QuranInstitute.Application;
using QuranInstitute.Domain.Entities.Users;
using QuranInstitute.Infrastructure;
using QuranInstitute.Infrastructure.Persistence.Context;
using QuranInstitute.Infrastructure.Persistence.Seed;

var builder = WebApplication.CreateBuilder(args);

// ─── لایه‌ها ────────────────────────────────────────
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// ─── Identity ───────────────────────────────────────
builder.Services
    .AddIdentity<AppUser, AppRole>(opt =>
    {
        opt.Password.RequiredLength = 6;
        opt.Password.RequireDigit = true;
        opt.Password.RequireUppercase = false;
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
});

// ─── MVC ────────────────────────────────────────────
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

// ─── Seeder ─────────────────────────────────────────
using (var scope = app.Services.CreateScope())
{
    try
    {
        await DatabaseSeeder.SeedAsync(scope.ServiceProvider);
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "خطا در Seeder هنگام راه‌اندازی");
    }
}

// ─── Pipeline ───────────────────────────────────────
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();