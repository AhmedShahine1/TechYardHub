using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechYardHub.Core.DTO.AuthViewModel.RoleModel
{
    [DebuggerDisplay("{RoleName,nq}")]
    public class RoleDTO
    {
        [Required(ErrorMessage = "Role Name is required"), Display(Name = "Role Name"), StringLength(50, ErrorMessage = "Role Name must be less than 50 characters")]
        public string RoleName { get; set; }

        [Required(ErrorMessage = "Role Description is required"), Display(Name = "Role Description"), StringLength(500, ErrorMessage = "Role Description must be less than 500 characters")]
        public string RoleDescription { get; set; }

        [Required(ErrorMessage = "Role Name in Arabic is required"), Display(Name = "Role Name Arabic"), StringLength(50, ErrorMessage = "Role Name Arabic must be less than 50 characters")]
        public string RoleAr { get; set; }
    }
}
