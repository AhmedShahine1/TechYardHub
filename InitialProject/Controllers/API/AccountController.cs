using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechYardHub.BusinessLayer.Interfaces;
using TechYardHub.Core.DTO;
using TechYardHub.Core.DTO.AuthViewModel;
using TechYardHub.Core.DTO.AuthViewModel.RegisterModel;
using TechYardHub.BusinessLayer.Services;
using TechYardHub.Core.Helpers;
using Microsoft.AspNetCore.Components.Forms.Mapping;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace TechYardHub.Controllers.API
{
    public class AccountController : BaseController
    {
        private readonly IAccountService _accountService;
        private readonly IMapper _mapper;
        public AccountController(IAccountService accountService, IMapper mapper)
        {
            _accountService = accountService;
            _mapper = mapper;
        }

        [HttpGet("Customer")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetCustomerDetails()
        {
            try
            {
                var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "").Trim();
                var customer = await _accountService.GetUserFromToken(token);
                if (customer == null)
                {
                    return NotFound(new BaseResponse
                    {
                        status = false,
                        ErrorCode = 404,
                        ErrorMessage = "Customer not found"
                    });
                }

                var authDto = _mapper.Map<AuthDTO>(customer);
                authDto.ProfileImage = await _accountService.GetUserProfileImage(customer.ProfileId);
                authDto.Role = await _accountService.GetUserRole(customer);
                return Ok(new BaseResponse
                {
                    status = true,
                    Data = authDto
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new BaseResponse
                {
                    status = false,
                    ErrorCode = 500,
                    ErrorMessage = "An unexpected error occurred.",
                    Data = ex.Message
                });
            }
        }

        [HttpPost("RegisterCustomer")]
        public async Task<IActionResult> RegisterCustomer([FromForm] RegisterCustomer model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new BaseResponse
                {
                    status = false,
                    ErrorCode = 400,
                    ErrorMessage = "Invalid model"
                });
            }

            try
            {
                var result = await _accountService.RegisterCustomer(model);

                if (result.Succeeded)
                {
                    return Ok(new BaseResponse
                    {
                        status = true,
                        ErrorCode = 200,
                        Data = model // Adjust if necessary
                    });
                }

                return BadRequest(new BaseResponse
                {
                    status = false,
                    ErrorCode = 500,
                    ErrorMessage = "User registration failed.",
                    Data = result.Errors.Select(e => e.Description).ToArray()
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new BaseResponse
                {
                    status = false,
                    ErrorCode = 500,
                    ErrorMessage = ex.Message
                });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromForm] LoginModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new BaseResponse
                {
                    status = false,
                    ErrorCode = 400,
                    ErrorMessage = "Invalid model"
                });
            }

            var result = await _accountService.Login(model);

            if (result.IsSuccess)
            {
                var user = await _accountService.GetUserFromToken(result.Token);
                var authDto = _mapper.Map<AuthDTO>(user);
                authDto.Token = result.Token;
                authDto.ProfileImage = await _accountService.GetUserProfileImage(user.ProfileId);
                authDto.Role = await _accountService.GetUserRole(user);
                return Ok(new BaseResponse
                {
                    status = true,
                    ErrorCode = 200,
                    Data = authDto
                });
            }
            if (result.Token == "405")
            {
                return BadRequest(new BaseResponse
                {
                    status = false,
                    ErrorCode = 405,
                    ErrorMessage = result.ErrorMessage
                });

            }
            return Unauthorized(new BaseResponse
            {
                status = false,
                ErrorCode = 401,
                ErrorMessage = result.ErrorMessage
            });
        }

        [HttpPost("logout")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> Logout()
        {
            try
            {
                var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var user = await _accountService.GetUserFromToken(token);
                var isSuccess = await _accountService.Logout(user);

                if (isSuccess)
                {
                    return Ok(new BaseResponse
                    {
                        status = true,
                        Data = "Successfully logged out"
                    });
                }

                return BadRequest(new BaseResponse
                {
                    status = false,
                    ErrorCode = 400,
                    ErrorMessage = "Logout failed"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new BaseResponse
                {
                    status = false,
                    ErrorCode = 500,
                    ErrorMessage = "An unexpected error occurred.",
                    Data = ex.Message
                });
            }
        }

        [HttpPost("Delete")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> DeleteAccount()
        {
            try
            {
                var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var customer = await _accountService.GetUserFromToken(token);

                if (customer == null)
                {
                    return NotFound(new BaseResponse
                    {
                        status = false,
                        ErrorCode = 404,
                        ErrorMessage = "Customer not found"
                    });
                }

                var result = await _accountService.Suspend(customer.Id);

                if (result.Succeeded)
                {
                    return Ok(new BaseResponse
                    {
                        status = true,
                        Data = "User account deleted successfully."
                    });
                }

                return BadRequest(new BaseResponse
                {
                    status = false,
                    ErrorCode = 400,
                    ErrorMessage = "Failed to delete account",
                    Data = result.Errors.Select(e => e.Description).ToArray()
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new BaseResponse
                {
                    status = false,
                    ErrorCode = 500,
                    ErrorMessage = "An unexpected error occurred.",
                    Data = ex.Message
                });
            }
        }
    }
}