using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Globalization;
using System.Threading.Tasks;
using WebApplication3.Data;
using Microsoft.AspNetCore.Http;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // Статическая переменная для хранения состояния аутентификации
    public static bool IsAuthenticated { get; set; } = false; // Изменили private set на public set

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddDbContext<SchoolDbContext>(options =>
            options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

        services.AddControllersWithViews()
            .AddViewLocalization()
            .AddDataAnnotationsLocalization();

        services.AddLocalization(options => options.ResourcesPath = "Resources");
        services.AddRazorPages();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        var supportedCultures = new[] { new CultureInfo("ru-RU") };
        app.UseRequestLocalization(new RequestLocalizationOptions
        {
            DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture("ru-RU"),
            SupportedCultures = supportedCultures,
            SupportedUICultures = supportedCultures
        });

        app.Use(async (context, next) =>
        {
            context.Request.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
            var form = await context.Request.ReadFormAsync();
            context.Request.Form = form;
            await next.Invoke();
        });

        // Middleware для проверки аутентификации
        app.Use(async (context, next) =>
        {
            // Разрешаем доступ к странице логина и AccessDenied без проверки
            if (context.Request.Path.StartsWithSegments("/Account/Login") ||
                context.Request.Path.StartsWithSegments("/Account/AccessDenied"))
            {
                await next.Invoke();
                return;
            }

            // Если пользователь не аутентифицирован, перенаправляем на страницу логина
            if (!IsAuthenticated)
            {
                context.Response.Redirect("/Account/Login");
                return;
            }

            await next.Invoke();
        });

        // Добавляем заголовок Cache-Control
        app.Use(async (context, next) =>
        {
            context.Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
            context.Response.Headers["Pragma"] = "no-cache";
            context.Response.Headers["Expires"] = "0";
            await next.Invoke();
        });

        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseRouting();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            endpoints.MapRazorPages();
        });
    }
}