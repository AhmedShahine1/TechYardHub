using AutoMapper;
using TechYardHub.BusinessLayer.Interfaces;
using TechYardHub.Core.DTO;
using TechYardHub.Core.DTO.AuthViewModel.RegisterModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechYardHub.BusinessLayer.Services;

namespace TechYardHub.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Policy = "Admin")]
    public class AdminController : Controller
    {
        private readonly IAccountService accountService;
        private readonly IMapper mapper;
        public AdminController(IAccountService _accountService, IMapper _mapper)
        {
            accountService = _accountService;
            mapper = _mapper;
        }

        [HttpGet]
        public IActionResult RegisterAdmin()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterAdmin(RegisterAdmin model)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, "Please correct the errors and try again.");
                return View(model);
            }

            try
            {
                var result = await accountService.RegisterAdmin(model);

                if (result.Succeeded)
                {
                    // Optionally, redirect to a success page or display a success message
                    TempData["SuccessMessage"] = "Registration successful!";
                    return RedirectToAction("Index", "Home"); // Adjust the redirection as needed
                }

                // If registration failed, display the errors
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return View(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var admin = await accountService.GetUserById(id);
            if (admin == null)
            {
                return NotFound();
            }
            var model = new RegisterAdmin
            {
                FullName = admin.FullName,
                Email = admin.Email
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await accountService.Suspend(id);
            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Home", new { area = "" });
            }
            return RedirectToAction("Index", "Admin");
        }
    }
}
