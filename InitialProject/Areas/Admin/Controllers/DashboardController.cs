using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechYardHub.BusinessLayer.Interfaces;
using TechYardHub.Core.DTO.AuthViewModel;

namespace TechYardHub.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Policy = "Admin")]
    public class DashboardController : Controller
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        // GET: Admin/Dashboard
        public async Task<IActionResult> Index()
        {
            var model = new DashboardViewModel
            {
                TotalCategories = await _dashboardService.GetTotalCategoriesAsync(),
                TotalAdmins = await _dashboardService.GetTotalAdminsAsync(),
                TotalCustomers = await _dashboardService.GetTotalCustomersAsync(),
                TotalProducts = await _dashboardService.GetTotalProductsAsync()
            };

            return View(model);
        }

        // GET: Admin/Dashboard/AdminUsers
        public async Task<IActionResult> AdminUsers()
        {
            var users = await _dashboardService.GetAllUsersExcludingRolesAsync(new List<string> { "Support Developer", "Customer" });
            return View(users);
        }

        // GET: Admin/Dashboard/CustomerUsers
        public async Task<IActionResult> CustomerUsers()
        {
            var users = await _dashboardService.GetAllUsersExcludingRolesAsync(new List<string> { "Support Developer", "Admin" });
            return View(users);
        }
    }
}
