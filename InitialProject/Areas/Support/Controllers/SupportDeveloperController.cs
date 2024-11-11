using AutoMapper;
using TechYardHub.BusinessLayer.Interfaces;
using TechYardHub.Core.DTO.AuthViewModel.RegisterModel;
using TechYardHub.Core.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TechYardHub.Core.DTO;

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
        public async Task<IActionResult> Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterSupportDeveloper model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await accountService.RegisterSupportDeveloper(model);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Index", "Home", new { area = "" });
                    }
                    return View(model);
                }
                return View(model);
            }
            catch (Exception ex)
            {
                var errorViewModel = new ErrorViewModel
                {
                    Message = "خطا في تسجيل البيانات",
                    StackTrace = ex.InnerException.Message
                };
                return View("~/Views/Shared/Error.cshtml", errorViewModel);
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
            var model = new RegisterSupportDeveloper
            {
                FullName = admin.FullName,
                Email = admin.Email,
            };
            return View(model);
        }

        //[HttpPost]
        //public async Task<IActionResult> Edit(string id, RegisterSupportDeveloper model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var result = await accountService.UpdateSupportDeveloper(id, model);
        //        if (result.Succeeded)
        //        {
        //            return RedirectToAction("Index", "Home", new { area = "" });
        //        }
        //        foreach (var error in result.Errors)
        //        {
        //            ModelState.AddModelError(string.Empty, error.Description);
        //        }
        //    }
        //    return View(model);
        //}

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
