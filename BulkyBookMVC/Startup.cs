using BulkyBook.DataAccess;
using BulkyBook.DataAccess.Data;
using BulkyBook.DataAccess.Initializer;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Stripe;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace BulkyBook.MVC;

public class Startup
{
    private const int _SESSION_IDDLE_TIMEOUT_IN_MINUTES = 30;

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(
                Configuration.GetConnectionString("BulkyBookConnection")));

        services.AddIdentity<IdentityUser, IdentityRole>().AddDefaultTokenProviders()
            .AddEntityFrameworkStores<ApplicationDbContext>();
        services.ConfigureApplicationCookie(options =>
        {
            options.LoginPath = $"/Identity/Account/Login";
            options.LogoutPath = $"/Identity/Account/Logout";
            options.AccessDeniedPath = $"/Identity/Account/AccessDenied";
        });
        services.AddAuthentication()
            .AddFacebook(options =>
            {
                options.AppId = Configuration.GetValue<string>("ExternalAuthentication:Facebook:AppId");
                options.AppSecret = Configuration.GetValue<string>("ExternalAuthentication:Facebook:AppSecret");
            })
            .AddGoogle(options =>
            {
                options.ClientId = Configuration["ExternalAuthentication:Google:ClientId"];
                options.ClientSecret = Configuration["ExternalAuthentication:Google:ClientSecret"];
            });

        services.AddSession(options =>
        {
            options.IdleTimeout = TimeSpan.FromMinutes(_SESSION_IDDLE_TIMEOUT_IN_MINUTES);
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
        });

        services.Configure<SendgridOptions>(Configuration.GetSection("EmailSender:Sendgrid"));
        services.Configure<TwilioOptions>(Configuration.GetSection("SmsSender:Twilio"));
        services.Configure<StripeOptions>(Configuration.GetSection("Payments:Stripe"));
        services.Configure<BrainTreeOptions>(Configuration.GetSection("Payments:BrainTree"));
        services.AddSingleton<IEmailSender, EmailSender>();
        services.AddSingleton<ITempDataProvider, CookieTempDataProvider>();
        services.AddSingleton<IBrainTreeGateway, BrainTreeGateway>();

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IDbInitializer, DbInitializer>();

        services.AddLocalization();

        services.AddControllersWithViews();
        services.AddRazorPages();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IDbInitializer dbInitializer)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/Home/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }
        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        StripeConfiguration.ApiKey = Configuration.GetSection("Payments:Stripe")["SecretKey"];

        app.UseSession();

        app.UseAuthentication();
        app.UseAuthorization();

        dbInitializer.Initialize();

        var supportedCultures = new List<CultureInfo>
        {
            new CultureInfo("pl-PL"),
            new CultureInfo("en-US")
        };

        var requestLocalizationOptions = new RequestLocalizationOptions
        {
            DefaultRequestCulture = new RequestCulture("pl-PL"),
            // Formatting numbers, dates, etc.
            SupportedCultures = supportedCultures,
            // UI strings that we have localized
            SupportedUICultures = supportedCultures
        };

        app.UseRequestLocalization(requestLocalizationOptions);

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllerRoute(
                name: "default",
                pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}");
            endpoints.MapRazorPages();
        });
    }
}