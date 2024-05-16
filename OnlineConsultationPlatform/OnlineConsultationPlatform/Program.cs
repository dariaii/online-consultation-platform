using System.Globalization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using OnlineConsultationPlatform.Core.Services;
using OnlineConsultationPlatform.Core.Infrastructure.Localization;
using OnlineConsultationPlatform.Data.DbContext;
using OnlineConsultationPlatform.Data.Entities;
using OnlineConsultationPlatform.Data.Repository;

const string DEFAULT_CULTURE = "bg";

var builder = WebApplication.CreateBuilder(args);

#region Configure Localization

builder.Services.AddSingleton<IStringLocalizerFactory, ResourcesLocalizerFactory>();
builder.Services.AddSingleton<IStringLocalizer, ResourcesLocalizer>();
builder.Services.AddLocalization();

#endregion Configure Localization

builder.Services
    .AddControllersWithViews()
    .AddViewLocalization()
    .AddDataAnnotationsLocalization();

builder.Services.AddDistributedMemoryCache();

#region Configure Data Layer

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlite(builder.Configuration.GetConnectionString("OnlineConsultationPlatformDb")));
builder.Services.AddScoped<IRepository, Repository>();

#endregion Configure Data Layer

#region Configure Authentication

builder.Services
    .AddIdentity<ApplicationUser, IdentityRole>()
    .AddRoles<IdentityRole>()
    .AddErrorDescriber<MultilanguageIdentityErrorDescriber>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddAuthentication();
builder.Services.AddAuthorization();

builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 1;

    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = true;
});

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.Name = ".OnlineConsultation.Identity";
    options.Cookie.Path = "/";
    options.Cookie.Domain = builder.Configuration["AuthenticationCookie:Domain"];
    options.ExpireTimeSpan = TimeSpan.FromMinutes(Convert.ToDouble(builder.Configuration["AuthenticationCookie:ExpirationTime"]));

    options.LoginPath = "/login";
    options.AccessDeniedPath = "/login";
    options.SlidingExpiration = true;
});

builder.Services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<IProfileService, ProfileService>();
builder.Services.AddScoped<IFileService, FileService>();
builder.Services.AddScoped<IMeetingService, MeetingService>();
builder.Services.AddScoped<IUsersService, UsersService>();
builder.Services.AddScoped<IReportsService, ReportsService>();
builder.Services.AddScoped(x =>
{
    var actionContext = x.GetRequiredService<IActionContextAccessor>().ActionContext;
    var factory = x.GetRequiredService<IUrlHelperFactory>();
    return factory.GetUrlHelper(actionContext);
});

#endregion Configure Authentication

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthentication().UseCookiePolicy();
app.UseAuthorization();

app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture(DEFAULT_CULTURE),
    SupportedCultures = [new CultureInfo(DEFAULT_CULTURE)],
    SupportedUICultures = [new CultureInfo(DEFAULT_CULTURE)],
    RequestCultureProviders = new List<IRequestCultureProvider> { new CookieRequestCultureProvider() }
});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

using (var scope = app.Services.CreateScope())
{
    DataSeeder.Initialize(
        scope.ServiceProvider.GetRequiredService<ApplicationDbContext>(),
        scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>(),
        builder.Configuration);
}

app.Run();