using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TechYardHub.BusinessLayer.Interfaces;
using TechYardHub.Core.DTO.AuthViewModel;
using TechYardHub.RepositoryLayer.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TechYardHub.Core.Entity.ApplicationData;

namespace TechYardHub.BusinessLayer.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public DashboardService(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<List<AuthDTO>> GetAllUsersExcludingRolesAsync(List<string> rolesToExclude)
        {
            var users = await _userManager.Users.ToListAsync();
            var usersExcludingRoles = new List<AuthDTO>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                if (!roles.Any(role => rolesToExclude.Contains(role)))
                {
                    usersExcludingRoles.Add(new AuthDTO
                    {
                        Id = user.Id,
                        FullName = user.FullName,
                        Email = user.Email,
                        PhoneNumber = user.PhoneNumber,
                        Role = roles.FirstOrDefault()
                    });
                }
            }

            return usersExcludingRoles;
        }

        public async Task<int> GetTotalCategoriesAsync() => await _unitOfWork.CategoriesRepository.CountAsync();

        public async Task<int> GetTotalAdminsAsync()
        {
            var adminRole = await _roleManager.FindByNameAsync("Admin");
            return adminRole == null ? 0 : _userManager.GetUsersInRoleAsync("Admin").Result.Count();
        }

        public async Task<int> GetTotalCustomersAsync()
        {
            var customerRole = await _roleManager.FindByNameAsync("Customer");
            return customerRole == null ? 0 : _userManager.GetUsersInRoleAsync("Customer").Result.Count();
        }

        public async Task<int> GetTotalProductsAsync() => await _unitOfWork.ProductsRepository.CountAsync();
    }
}
