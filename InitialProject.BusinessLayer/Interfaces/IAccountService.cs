using TechYardHub.Core.DTO.AuthViewModel;
using TechYardHub.Core.DTO.AuthViewModel.RegisterModel;
using TechYardHub.Core.DTO.AuthViewModel.RoleModel;
using TechYardHub.Core.Entity.ApplicationData;
using TechYardHub.Core.Entity.Files;
using Microsoft.AspNetCore.Identity;

namespace TechYardHub.BusinessLayer.Interfaces;

public interface IAccountService
{
    Task<ApplicationUser> GetUserById(string id);
    Task<ApplicationUser> FindByEmailAsync(string email);
    Task<ApplicationUser> FindByPhoneNumberAsync(string phoneNumber);
    Task<IdentityResult> RegisterAdmin(RegisterAdmin model);
    Task<IdentityResult> RegisterSupportDeveloper(RegisterSupportDeveloper model);
    Task<IdentityResult> RegisterCustomer(RegisterCustomer model);
    Task<(bool IsSuccess, string Token, string ErrorMessage)> Login(LoginModel model);
    Task<bool> Logout(ApplicationUser user);
    Task<ApplicationUser> GetUserFromToken(string token);
    Task<string> AddRoleAsync(RoleUserModel model);
    Task<List<string>> GetRoles();
    Task<string> GetUserRole(ApplicationUser user);
    Task<string> GetUserProfileImage(string profileId);
    Task<Paths> GetPathByName(string name);
    string ValidateJwtToken(string token);
    int GenerateRandomNo();
    ////------------------------------------------------------
    Task<IdentityResult> Activate(string userId);
    Task<IdentityResult> Suspend(string userId);
}