using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TechYardHub.BusinessLayer.Interfaces;
using TechYardHub.Core.DTO.AuthViewModel.CategoryModel;
using TechYardHub.Core.DTO.AuthViewModel.ProductModel;
using TechYardHub.Core.Entity.CategoryData;
using TechYardHub.Core.Entity.Files;
using TechYardHub.Core.Entity.ProductData;
using TechYardHub.RepositoryLayer.Interfaces;

namespace TechYardHub.BusinessLayer.Services
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileHandling _fileHandling;
        private readonly ICategoryService _categoryService;
        private readonly IMapper _mapper;
        private readonly Paths _productPath;

        public ProductService(IUnitOfWork unitOfWork, IFileHandling fileHandling, IMapper mapper, ICategoryService categoryService)
        {
            _unitOfWork = unitOfWork;
            _fileHandling = fileHandling;
            _mapper = mapper;
            _productPath = unitOfWork.PathsRepository.Find(a => a.Name == "ProductImages");
            _categoryService = categoryService;
        }

        // Helper method to map Product to ProductDto with image URLs
        private async Task<ProductDto> MapProductToDtoAsync(Product product)
        {
            var productDto = _mapper.Map<ProductDto>(product);

            // Fetch image URLs if images exist
            if (product.Images != null && product.Images.Any())
            {
                productDto.ImageUrls = new List<string>();
                foreach (var image in product.Images)
                {
                    productDto.ImageUrls.Add(await _fileHandling.GetFile(image.Id));
                }
            }

            return productDto;
        }

        public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
        {
            try
            {
                // Fetch products with associated images and categories included
                var products = await _unitOfWork.ProductsRepository.GetAllAsync(
                    include: query => query.Include(p => p.Images).Include(p => p.Category));

                if (products == null || !products.Any())
                {
                    return new List<ProductDto>();
                }

                var productDtos = new List<ProductDto>();
                foreach (var product in products)
                {
                    var productDto = await MapProductToDtoAsync(product);
                    productDtos.Add(productDto);
                }

                return productDtos;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while fetching products.", ex);
            }
        }

        public async Task<ProductDto> GetProductByIdAsync(string id)
        {
            try
            {
                // Fetch product with images and category included
                var product = await _unitOfWork.ProductsRepository.FindAsync(p => p.Id == id,
                    include: query => query.Include(p => p.Images).Include(p => p.Category));

                if (product == null)
                {
                    throw new KeyNotFoundException($"Product with ID {id} not found.");
                }

                return await MapProductToDtoAsync(product);
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while fetching product with ID {id}.", ex);
            }
        }

        public async Task<ProductDto> CreateProductAsync(ProductDto productDto)
        {
            try
            {
                var product = _mapper.Map<Product>(productDto);

                // Upload product images if provided
                if (productDto.Images != null && productDto.Images.Any())
                {
                    var imageIds = new List<string>();
                    foreach (var image in productDto.Images)
                    {
                        string imageId = await _fileHandling.UploadFile(image, _productPath);
                        imageIds.Add(imageId);
                    }

                    // Store uploaded images in the product
                    product.Images = imageIds.Select(id => _unitOfWork.ImagesRepository.GetById(id)).ToList();
                }

                // Handle category (assuming CategoryId is provided in the DTO)
                var category = await _unitOfWork.CategoriesRepository.GetByIdAsync(productDto.CategoryId);
                if (category == null)
                {
                    throw new KeyNotFoundException($"Category with ID {productDto.CategoryId} not found.");
                }
                product.Category = category;

                await _unitOfWork.ProductsRepository.AddAsync(product);
                await _unitOfWork.SaveChangesAsync();

                return _mapper.Map<ProductDto>(product);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while creating the product.", ex);
            }
        }

        public async Task<ProductDto> UpdateProductAsync(ProductDto productDto)
        {
            try
            {
                var product = await _unitOfWork.ProductsRepository.GetByIdAsync(productDto.Id);
                if (product == null)
                {
                    throw new KeyNotFoundException($"Product with ID {productDto.Id} not found.");
                }

                // Update product details
                _mapper.Map(productDto, product);

                // Optionally, update images if provided
                if (productDto.Images != null && productDto.Images.Any())
                {
                    foreach (var image in productDto.Images)
                    {
                        // Upload new image if provided
                        string imageId = await _fileHandling.UploadFile(image, _productPath);
                        product.Images.Add(new Images { Id = imageId });
                    }
                }

                // Update category if necessary
                if (!string.IsNullOrEmpty(productDto.CategoryId))
                {
                    var category = await _unitOfWork.CategoriesRepository.GetByIdAsync(productDto.CategoryId);
                    if (category == null)
                    {
                        throw new KeyNotFoundException($"Category with ID {productDto.CategoryId} not found.");
                    }
                    product.Category = category;
                }

                _unitOfWork.ProductsRepository.Update(product);
                await _unitOfWork.SaveChangesAsync();

                return _mapper.Map<ProductDto>(product);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while updating the product.", ex);
            }
        }

        public async Task<bool> DeleteProductAsync(string id)
        {
            try
            {
                var product = await _unitOfWork.ProductsRepository.FindAsync(a=>a.Id == id ,include:query=>query.Include(p=>p.Images));
                if (product == null)
                {
                    throw new KeyNotFoundException($"Product with ID {id} not found.");
                }
                var imagesToDelete = product.Images?.ToList(); // Make a copy
                // Optionally delete product images
                if (product.Images != null && product.Images.Any())
                {
                    foreach (var image in imagesToDelete)
                    {
                        await _fileHandling.DeleteFile(image.Id);
                    }
                }

                _unitOfWork.ProductsRepository.Delete(product);
                await _unitOfWork.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while deleting the product.", ex);
            }
        }

        public async Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync()
        {
            return await _categoryService.GetAllCategoriesAsync();
        }
    }
}