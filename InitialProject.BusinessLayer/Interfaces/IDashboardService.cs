using TechYardHub.Core.DTO.AuthViewModel;
using TechYardHub.Core.Entity.ApplicationData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechYardHub.BusinessLayer.Interfaces
{
    public interface IDashboardService
    {
        Task<IEnumerable<AuthDTO>> GetAllUsersAsync();
    }
}
