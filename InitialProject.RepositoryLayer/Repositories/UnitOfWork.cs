﻿using Microsoft.AspNetCore.Identity;
using TechYardHub.Core;
using TechYardHub.Core.Entity.ApplicationData;
using TechYardHub.Core.Entity.CategoryData;
using TechYardHub.Core.Entity.Files;
using TechYardHub.Core.Entity.ProductData;
using TechYardHub.RepositoryLayer.Interfaces;

namespace TechYardHub.RepositoryLayer.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    public IBaseRepository<ApplicationUser> UserRepository { get; set; }
    public IBaseRepository<ApplicationRole> RoleRepository { get; set; }
    public IBaseRepository<IdentityUserRole<string>> UserRoleRepository { get; set; }

    public IBaseRepository<Paths> PathsRepository { get; set; }
    public IBaseRepository<Images> ImagesRepository { get; set; }
    public IBaseRepository<Category> CategoriesRepository { get; set; }
    public IBaseRepository<Product> ProductsRepository { get; set; }
    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
        UserRepository = new BaseRepository<ApplicationUser>(context);
        RoleRepository = new BaseRepository<ApplicationRole>(context);
        UserRoleRepository = new BaseRepository<IdentityUserRole<string>>(context);
        PathsRepository = new BaseRepository<Paths>(context);
        ImagesRepository = new BaseRepository<Images>(context);
        CategoriesRepository = new BaseRepository<Category>(context);
        ProductsRepository = new BaseRepository<Product>(context);
    }

    public int SaveChanges()
    {
        return _context.SaveChanges();
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}