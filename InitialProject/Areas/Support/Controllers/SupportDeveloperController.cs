using AutoMapper;
using TechYardHub.BusinessLayer.Interfaces;
using TechYardHub.Core.DTO.AuthViewModel.RegisterModel;
using TechYardHub.Core.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TechYardHub.Areas.Support.Controllers
{
    [Area("Support")]
    [Authorize(Policy = "Support Developer")]
    public class SupportDeveloperController : Controller
    {
        private readonly IAccountService accountService;
        private readonly IMapper mapper;
        public SupportDeveloperController(IAccountService _accountService, IMapper _mapper)
        {
            accountService = _accountService;
            mapper = _mapper;
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterSupportDeveloper model)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, "Please correct the errors and try again.");
                return View(model);
            }

            try
            {
                // Check for duplicate email or phone number
                var existingUser = await accountService.FindByEmailAsync(model.Email);
                if (existingUser != null)
                {
                    ModelState.AddModelError("Email", "An account with this email already exists.");
                    return View(model);
                }

                var existingPhoneUser = await accountService.FindByPhoneNumberAsync(model.PhoneNumber);
                if (existingPhoneUser != null)
                {
                    ModelState.AddModelError("PhoneNumber", "An account with this phone number already exists.");
                    return View(model);
                }

                var result = await accountService.RegisterSupportDeveloper(model);

                if (result.Succeeded)
                {
                    TempData["SuccessMessage"] = "Registration successful!";
                    return RedirectToAction("Index", "Home"); // Adjust redirection as needed
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
                ModelState.AddModelError(string.Empty, "An error occurred while processing your request. Please try again later.");
                // Log the exception details for further analysis
                Console.WriteLine(ex); // Replace with a logging service
                return View(model);
            }
        }
    }
}
