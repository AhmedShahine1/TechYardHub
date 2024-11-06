
using AutoMapper;
using TechYardHub.Core.DTO.AuthViewModel;
using TechYardHub.Core.DTO.AuthViewModel.FilesModel;
using TechYardHub.Core.DTO.AuthViewModel.RegisterModel;
using TechYardHub.Core.DTO.AuthViewModel.RoleModel;
using TechYardHub.Core.Entity.ApplicationData;
using TechYardHub.Core.Entity.Files;

namespace TechYardHub.BusinessLayer.AutoMapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            //--------------------------------------------------------------------------------------------------------
            // Mapping for RoleDTO <-> ApplicationRole
            CreateMap<RoleDTO, ApplicationRole>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.RoleName))
                .ForMember(dest => dest.ArName, opt => opt.MapFrom(src => src.RoleAr))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.RoleDescription))
                .ReverseMap();

            //--------------------------------------------------------------------------------------------------------
            // Mapping for Paths <-> PathsModel
            CreateMap<Paths, PathsModel>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ReverseMap();

            //--------------------------------------------------------------------------------------------------------
            // Mapping for ApplicationUser <-> RegisterSupportDeveloper
            CreateMap<RegisterSupportDeveloper, ApplicationUser>()
               .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email))
               .ForMember(dest => dest.Profile, opt => opt.Ignore()) 
               .ReverseMap()
               .ForMember(dest => dest.ImageProfile, opt => opt.Ignore());
            //--------------------------------------------------------------------------------------------------------
            // Mapping for ApplicationUser <-> RegisterAdmin
            CreateMap<RegisterAdmin, ApplicationUser>()
               .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email))
               .ForMember(dest => dest.Profile, opt => opt.Ignore()) 
               .ReverseMap()
               .ForMember(dest => dest.ImageProfile, opt => opt.Ignore());
            //--------------------------------------------------------------------------------------------------------
            // Mapping for ApplicationUser <-> RegisterCustomer
            CreateMap<RegisterCustomer, ApplicationUser>()
               .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email))
               .ForMember(dest => dest.Profile, opt => opt.Ignore()) 
               .ReverseMap()
               .ForMember(dest => dest.ImageProfile, opt => opt.Ignore());

        }
    }
}
