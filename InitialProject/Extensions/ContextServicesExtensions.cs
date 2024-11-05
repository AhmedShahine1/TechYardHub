using System.Text.Json.Serialization;
using TechYardHub.BusinessLayer.Interfaces;
using TechYardHub.BusinessLayer.Services;
using TechYardHub.Core;
using TechYardHub.RepositoryLayer.Interfaces;
using TechYardHub.RepositoryLayer.Repositories;
using TechYardHub.BusinessLayer.AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace TechYardHub.Extensions;

public static class ContextServicesExtensions
{
    public static IServiceCollection AddContextServices(this IServiceCollection services, IConfiguration config)
    {

        //- context && json services
        services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(config.GetConnectionString("DefaultConnection")));//,b => b.MigrationsAssembly(typeof(ApplicationContext).Assembly.FullName)).UseLazyLoadingProxies());
        services.AddControllers().AddJsonOptions(x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
        services.AddControllersWithViews();
        services.AddSingleton<IRequestResponseService, RequestResponseService>();
        // IBaseRepository && IUnitOfWork Service
        //services.AddTransient(typeof(IBaseRepository<>), typeof(BaseRepository<>)); // only Repository
        services.AddTransient<IUnitOfWork, UnitOfWork>(); // Repository and UnitOfWork
        services.AddAutoMapper(typeof(MappingProfile));

        return services;
    }

}
