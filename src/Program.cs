using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using UserManagement.Services;
using UserManagement.Services.Interfaces;
using UserManagement.Models;
using UserManagement.Data;
using UserManagement.Controllers.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        ConfigureServices(builder.Services, builder.Configuration);
        var app = builder.Build();
        Configure(app);
        app.Run();
    }

    private static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<UserManagementDbContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
        });

        services.AddTransient<IUserManagerService, UserManagerService>();
        services.AddTransient<IAccountService, AccountService>();
        services.AddTransient<IUserApiController, UserApiController>();

        services.AddControllersWithViews();
        services.AddHttpContextAccessor();

        // User authentication based on cookies
        services.AddAuthentication("UserAccount")
            .AddCookie("UserAccount", options =>
            {
                options.Cookie.Name = "UserCookie";
                options.LoginPath = "/";
            });

        // Admin authentication based on cookies
        services.AddAuthentication("AdminAccount")
            .AddCookie("AdminAccount", options =>
            {
                options.Cookie.Name = "AdminCookie";
                options.LoginPath = "/Admin";
            });
    }

    private static void Configure(WebApplication app)
    {
        app.UseDefaultFiles();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseCookiePolicy();
        app.UseAuthentication();
        app.UseAuthorization();

        // routing for users
        app.MapControllerRoute("default", "{controller=Account}/{action=Index}/{id?}");

        // routing for the admin
        app.MapControllerRoute(
            name: "Admin",
            pattern: "{area:exists}/{controller=AdminPanel}/{action=Dashboard}/{id?}");
    }
}
