using Microsoft.AspNetCore.Http;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TechYardHub.Core.DTO.AuthViewModel.RegisterModel
{
    public class RegisterAdmin
    {
        [DisplayName("Full Name")]
        [Required(ErrorMessage = "You should enter the Full Name"), StringLength(100)]
        public string FullName { get; set; }

        [DisplayName("Email")]
        [DataType(DataType.EmailAddress)]
        [Required(ErrorMessage = "You should enter the Email")]
        public string Email { get; set; }
        
        [DisplayName("PhoneNumber")]
        [DataType(DataType.PhoneNumber)]
        [Required(ErrorMessage = "You should enter the PhoneNumber")]
        public string PhoneNumber { get; set; }

        [DisplayName("Password")]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "You should enter the Password")]
        public string Password { get; set; }

        [DisplayName("Confirm Password")]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "You should confirm the Password")]
        [Compare("Password", ErrorMessage = "Password and confirmation do not match")]
        public string ConfirmPassword { get; set; }

        [DisplayName("Files Image")]
        public IFormFile? ImageProfile { get; set; }
    }
}
