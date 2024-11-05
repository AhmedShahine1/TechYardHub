using TechYardHub.Core.DTO.AuthViewModel.PostsModel;
using TechYardHub.Core.DTO.AuthViewModel;
using TechYardHub.Core.Entity.ApplicationData;
using TechYardHub.Core.Entity.Posts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechYardHub.BusinessLayer.Interfaces
{
    public interface IDashboardService
    {
        int GetCompanyUserCountAsync();
        int GetEmployeeUserCountAsync();
        int GetUsersWithoutCompanyCountAsync();
        Task<int> GetPostCountAsync();
        Task<IEnumerable<AuthDTO>> GetAllUsersAsync();
        Task<IEnumerable<AuthDTO>> GetAllCompaniesAsync();
        Task<IEnumerable<AuthDTO>> GetAllEmployeesAsync();
        Task<IEnumerable<PostDTO>> GetAllPostsAsync();
    }
}
