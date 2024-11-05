using TechYardHub.Core.Entity.Files;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;

namespace TechYardHub.Core.Entity.ApplicationData
{
    [DebuggerDisplay("{FullName,nq}")]
    public class ApplicationUser : IdentityUser
    {
        public bool Status { get; set; } = true; // يدل على ما إذا كان الحساب نشطًا أم لا.
        public string FullName { get; set; }
        public override string? Email { get; set; }
        public DateTime RegistrationDate { get; set; } = DateTime.Now;
        [ForeignKey(nameof(Profile))]
        public string ProfileId { get; set; }
        public Images Profile { get; set; } // صورة الملف الشخصي للمستخدم.
    }
}
