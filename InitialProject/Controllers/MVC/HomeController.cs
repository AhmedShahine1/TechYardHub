using TechYardHub.BusinessLayer.Interfaces;
using TechYardHub.BusinessLayer.Services;
using TechYardHub.Core.DTO.AuthViewModel;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TechYardHub.Controllers.MVC
{
    [Authorize(Policy = "Admin")]
    public class HomeController : Controller
    {
        private readonly IDashboardService _dashboardService;

        public HomeController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        public async Task<IActionResult> Index()
        {
            var viewModel = new DashboardViewModel
            {
            };

            return View(viewModel);
        }

        // Get all Users
        public async Task<IActionResult> Users()
        {
            var users = await _dashboardService.GetAllUsersAsync();
            return View(users);
        }
    }
}
