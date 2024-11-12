using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

public class RegisterAdmin
{
    [DisplayName("Full Name")]
    [Required(ErrorMessage = "You should enter the Full Name")]
    [StringLength(100, ErrorMessage = "Full Name cannot exceed 100 characters")]
    public string FullName { get; set; }

    [DisplayName("Email")]
    [DataType(DataType.EmailAddress)]
    [Required(ErrorMessage = "You should enter the Email")]
    [EmailAddress(ErrorMessage = "Invalid Email Format")]
    public string Email { get; set; }

    [DisplayName("PhoneNumber")]
    [DataType(DataType.PhoneNumber)]
    [Required(ErrorMessage = "You should enter the PhoneNumber")]
    [Phone(ErrorMessage = "Invalid Phone Number")]
    public string PhoneNumber { get; set; }

    [DisplayName("Password")]
    [DataType(DataType.Password)]
    [Required(ErrorMessage = "You should enter the Password")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters long")]
    public string Password { get; set; }

    [DisplayName("Confirm Password")]
    [DataType(DataType.Password)]
    [Required(ErrorMessage = "You should confirm the Password")]
    [Compare("Password", ErrorMessage = "Password and confirmation do not match")]
    public string ConfirmPassword { get; set; }

    [DisplayName("Profile Image")]
    public IFormFile? ImageProfile { get; set; }
}
