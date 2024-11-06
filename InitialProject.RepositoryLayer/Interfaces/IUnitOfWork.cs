using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TechYardHub.Core.Entity.ApplicationData;
using TechYardHub.Core.Entity.Files;

namespace TechYardHub.RepositoryLayer.Interfaces;

public interface IUnitOfWork : IDisposable
{
    public IBaseRepository<ApplicationUser> UserRepository { get; }
    public IBaseRepository<ApplicationRole> RoleRepository { get; }
    public IBaseRepository<IdentityUserRole<string>> UserRoleRepository { get; }
    public IBaseRepository<Paths> PathsRepository { get; }
    public IBaseRepository<Images> ImagesRepository { get; }

    //-----------------------------------------------------------------------------------
    int SaveChanges();

    Task<int> SaveChangesAsync();
}