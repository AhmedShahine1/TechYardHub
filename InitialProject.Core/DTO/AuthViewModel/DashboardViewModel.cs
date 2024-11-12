using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechYardHub.Core.DTO.AuthViewModel
{
    public class DashboardViewModel
    {
        public int TotalCategories { get; set; }
        public int TotalAdmins { get; set; }
        public int TotalCustomers { get; set; }
        public int TotalProducts { get; set; }
    }
}
