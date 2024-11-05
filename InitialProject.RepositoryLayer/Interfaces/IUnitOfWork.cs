using Microsoft.EntityFrameworkCore;

namespace TechYardHub.RepositoryLayer.Interfaces;

public interface IUnitOfWork : IDisposable
{
    //-----------------------------------------------------------------------------------
    int SaveChanges();

    Task<int> SaveChangesAsync();
}