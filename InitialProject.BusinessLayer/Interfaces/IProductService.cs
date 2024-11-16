using Microsoft.AspNetCore.Http;
using TechYardHub.Core.DTO.AuthViewModel.CategoryModel;
using TechYardHub.Core.DTO.AuthViewModel.ProductModel;

namespace TechYardHub.BusinessLayer.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDto>> GetAllProductsAsync();
        Task<ProductDto> GetProductByIdAsync(string id);
        Task<ProductDto> CreateProductAsync(ProductDto productDto);
        Task<ProductDto> UpdateProductAsync(ProductDto productDto);
        Task<ProductDto> UpdateStatusProductAsync(string id);
        Task<bool> DeleteProductAsync(string id);
        Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync();
        Task AddProductsFromExcelAsync(IFormFile excelFile);
    }
}
