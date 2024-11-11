using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        Task<bool> DeleteProductAsync(string id);
        Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync();
    }
}
