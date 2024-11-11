
using AutoMapper;
using TechYardHub.Core.DTO.AuthViewModel.CategoryModel;
using TechYardHub.Core.DTO.AuthViewModel.FilesModel;
using TechYardHub.Core.DTO.AuthViewModel.ProductModel;
using TechYardHub.Core.DTO.AuthViewModel.RegisterModel;
using TechYardHub.Core.DTO.AuthViewModel.RoleModel;
using TechYardHub.Core.Entity.ApplicationData;
using TechYardHub.Core.Entity.CategoryData;
using TechYardHub.Core.Entity.Files;
using TechYardHub.Core.Entity.ProductData;

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
            //--------------------------------------------------------------------------------------------------------
            // Mapping for CategoryDto <-> Category
            CreateMap<Category, CategoryDto>()
                .ForMember(dest => dest.Image, opt => opt.Ignore())
                .ForMember(dest => dest.ImageUrl, opt => opt.Ignore());

            CreateMap<CategoryDto, Category>()
                .ForMember(dest => dest.image, opt => opt.Ignore());
            //--------------------------------------------------------------------------------------------------------
            // Mapping from Product entity to ProductDto
            CreateMap<Product, ProductDto>()
                .ForMember(dest => dest.Images, opt => opt.Ignore()) // Ignoring file upload (handled separately)
                .ForMember(dest => dest.ImageUrls, opt => opt.Ignore())
                .ForMember(dest => dest.Processors, opt => opt.MapFrom(src => src.Processors != null ? src.Processors : new List<string>()))
                .ForMember(dest => dest.RAM, opt => opt.MapFrom(src => src.RAM != null ? src.RAM : new List<string>()))
                .ForMember(dest => dest.Storage, opt => opt.MapFrom(src => src.Storage != null ? src.Storage : new List<string>()))
                .ForMember(dest => dest.GraphicsCards, opt => opt.MapFrom(src => src.GraphicsCards != null ? src.GraphicsCards : new List<string>()))
                .ForMember(dest => dest.ScreenSizes, opt => opt.MapFrom(src => src.ScreenSizes != null ? src.ScreenSizes : new List<string>()))
                .ForMember(dest => dest.BatteryLives, opt => opt.MapFrom(src => src.BatteryLives != null ? src.BatteryLives : new List<string>()))
                .ForMember(dest => dest.OperatingSystems, opt => opt.MapFrom(src => src.OperatingSystems != null ? src.OperatingSystems : new List<string>()))
                .ForMember(dest => dest.Ports, opt => opt.MapFrom(src => src.Ports != null ? src.Ports : new List<string>()));

            // Mapping from ProductDto to Product entity
            CreateMap<ProductDto, Product>()
                .ForMember(dest => dest.Images, opt => opt.Ignore()) // Ignoring collection of Images (handled separately)
                .ForMember(dest => dest.Processors, opt => opt.MapFrom(src => src.Processors ?? new List<string>()))
                .ForMember(dest => dest.RAM, opt => opt.MapFrom(src => src.RAM ?? new List<string>()))
                .ForMember(dest => dest.Storage, opt => opt.MapFrom(src => src.Storage ?? new List<string>()))
                .ForMember(dest => dest.GraphicsCards, opt => opt.MapFrom(src => src.GraphicsCards ?? new List<string>()))
                .ForMember(dest => dest.ScreenSizes, opt => opt.MapFrom(src => src.ScreenSizes ?? new List<string>()))
                .ForMember(dest => dest.BatteryLives, opt => opt.MapFrom(src => src.BatteryLives ?? new List<string>()))
                .ForMember(dest => dest.OperatingSystems, opt => opt.MapFrom(src => src.OperatingSystems ?? new List<string>()))
                .ForMember(dest => dest.Ports, opt => opt.MapFrom(src => src.Ports ?? new List<string>()));
        }
    }
}
