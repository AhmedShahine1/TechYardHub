using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TechYardHub.Core.Entity.ApplicationData;
using TechYardHub.Core.Entity.CategoryData;
using TechYardHub.Core.Entity.Files;
using TechYardHub.Core.Entity.ProductData;

namespace TechYardHub.RepositoryLayer.Interfaces;

public interface IUnitOfWork : IDisposable
{
    public IBaseRepository<ApplicationUser> UserRepository { get; }
    public IBaseRepository<ApplicationRole> RoleRepository { get; }
    public IBaseRepository<IdentityUserRole<string>> UserRoleRepository { get; }
    public IBaseRepository<Paths> PathsRepository { get; }
    public IBaseRepository<Images> ImagesRepository { get; }
    public IBaseRepository<Category> CategoriesRepository { get; }
    public IBaseRepository<Product> ProductsRepository { get; }

    //-----------------------------------------------------------------------------------
    int SaveChanges();

    Task<int> SaveChangesAsync();
}