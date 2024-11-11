using TechYardHub.BusinessLayer.AutoMapper;
using TechYardHub.BusinessLayer.Interfaces;
using TechYardHub.BusinessLayer.Services;

namespace TechYardHub.Extensions;

public static class ApplicationServicesExtensions
{
    // interfaces sevices [IAccountService, IPhotoHandling  ]
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
    {

        services.AddDistributedMemoryCache(); // Add this line to configure the distributed cache

        // Session Service
        services.AddSession(options =>
        {
            options.IdleTimeout = TimeSpan.FromHours(12);
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
        });
        services.AddScoped<IAccountService, AccountService>();
        services.AddTransient<IFileHandling, FileHandling>();
        services.AddTransient<IDashboardService, DashboardService>();
        services.AddTransient<ICategoryService, CategoryService>();
        services.AddTransient<IProductService, ProductService>();
        services.AddAutoMapper(typeof(MappingProfile));

        services.AddHttpClient();
        return services;
    }

    public static IApplicationBuilder UseApplicationMiddleware(this IApplicationBuilder app)
    {
        app.UseSession();
        /*   app.UseHangfireDashboard("/Hangfire/Dashboard");

           app.UseWebSockets();*/

        return app;
    }
}
