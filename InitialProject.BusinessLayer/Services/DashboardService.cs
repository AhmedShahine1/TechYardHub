using TechYardHub.BusinessLayer.Interfaces;
using TechYardHub.Core.DTO.AuthViewModel;
using TechYardHub.RepositoryLayer.Interfaces;

namespace TechYardHub.BusinessLayer.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAccountService _accountService;
        private readonly IFileHandling _fileService;

        public DashboardService(IUnitOfWork unitOfWork, IAccountService accountService, IFileHandling fileService)
        {
            _unitOfWork = unitOfWork;
            _accountService = accountService;
            _fileService = fileService;
        }

        public async Task<IEnumerable<AuthDTO>> GetAllUsersAsync()
        {
            var users = await _unitOfWork.UserRepository.GetAllAsync();
            return users.Select(user => new AuthDTO
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                ProfileImage = _accountService.GetUserProfileImage(user.ProfileId).Result,
            });
        }
    }
}
