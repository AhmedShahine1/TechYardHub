using TechYardHub.Core.Helpers;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechYardHub.Core.DTO.AuthViewModel.RegisterModel
{
    public class RegisterAdmin
    {
        [DisplayName("Full Name")]
        [Required(ErrorMessage = "You should enter the Full Name"), StringLength(100)]
        public string FullName { get; set; }

        [DisplayName("EmailorPhoneNumber")]
        [DataType(DataType.EmailAddress)]
        [Required(ErrorMessage = "You should enter the EmailorPhoneNumber")]
        public string Email { get; set; }

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
