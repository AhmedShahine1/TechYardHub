using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml.Drawing;
using OfficeOpenXml;
using TechYardHub.BusinessLayer.Interfaces;
using TechYardHub.Core.DTO.AuthViewModel.CategoryModel;
using TechYardHub.Core.DTO.AuthViewModel.ProductModel;
using TechYardHub.Core.Entity.Files;
using TechYardHub.Core.Entity.ProductData;
using TechYardHub.RepositoryLayer.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using TechYardHub.Core.Helpers;

namespace TechYardHub.BusinessLayer.Services
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileHandling _fileHandling;
        private readonly ICategoryService _categoryService;
        private readonly IMapper _mapper;
        private readonly Paths _productPath;
        private readonly IMemoryCache _memoryCache;

        public ProductService(IUnitOfWork unitOfWork, IFileHandling fileHandling, IMapper mapper,
                              ICategoryService categoryService, IMemoryCache memoryCache)
        {
            _unitOfWork = unitOfWork;
            _fileHandling = fileHandling;
            _mapper = mapper;
            _memoryCache = memoryCache;
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
            productDto.Category = await _categoryService.GetCategoryByIdAsync(product.CategoryId);
            return productDto;
        }

        public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
        {
            try
            {
                // Check if products are cached
                if (!_memoryCache.TryGetValue(CacheMemory.Product, out IEnumerable<ProductDto> cachedProducts))
                {
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

                    // Cache the products
                    _memoryCache.Set(CacheMemory.Product, productDtos, TimeSpan.FromMinutes(30));
                    cachedProducts = productDtos;
                }

                return cachedProducts;
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
                // Cache key for individual products
                var cacheKey = $"{CacheMemory.Product}_{id}";

                if (!_memoryCache.TryGetValue(cacheKey, out ProductDto cachedProduct))
                {
                    var product = await _unitOfWork.ProductsRepository.FindAsync(p => p.Id == id,
                        include: query => query.Include(p => p.Images).Include(p => p.Category));

                    if (product == null)
                    {
                        throw new KeyNotFoundException($"Product with ID {id} not found.");
                    }

                    cachedProduct = await MapProductToDtoAsync(product);
                    _memoryCache.Set(cacheKey, cachedProduct, TimeSpan.FromMinutes(30));
                }

                return cachedProduct;
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

                // Handle product (assuming CategoryId is provided in the DTO)
                var category = await _unitOfWork.CategoriesRepository.GetByIdAsync(productDto.CategoryId);
                if (category == null)
                {
                    throw new KeyNotFoundException($"Category with ID {productDto.CategoryId} not found.");
                }
                product.Category = category;

                await _unitOfWork.ProductsRepository.AddAsync(product);
                await _unitOfWork.SaveChangesAsync();
                // Invalidate cache
                _memoryCache.Remove(CacheMemory.Product);

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

                _mapper.Map(productDto, product);

                _unitOfWork.ProductsRepository.Update(product);
                await _unitOfWork.SaveChangesAsync();

                // Invalidate cache
                _memoryCache.Remove(CacheMemory.Product);
                _memoryCache.Remove($"{CacheMemory.Product}_{productDto.Id}");

                return _mapper.Map<ProductDto>(product);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while updating the product.", ex);
            }
        }

        public async Task<ProductDto> UpdateStatusProductAsync(string id)
        {
            try
            {
                var product = await _unitOfWork.ProductsRepository.GetByIdAsync(id);
                if (product == null)
                {
                    // Handle case where product does not exist
                    throw new KeyNotFoundException($"Category with ID {id} not found.");
                }
                product.Status = (product.Status) ? false : true;
                _unitOfWork.ProductsRepository.Update(product);
                await _unitOfWork.SaveChangesAsync();
                // Invalidate cache
                _memoryCache.Remove(CacheMemory.Product);
                _memoryCache.Remove($"{CacheMemory.Product}_{product.Id}");

                return _mapper.Map<ProductDto>(product);
            }
            catch (Exception ex)
            {
                // Log the error and rethrow or handle it gracefully
                throw new Exception("An error occurred while updating the product.", ex);
            }
        }

        public async Task<bool> DeleteProductAsync(string id)
        {
            try
            {
                var product = await _unitOfWork.ProductsRepository.FindAsync(a => a.Id == id, include: query => query.Include(p => p.Images));
                if (product == null)
                {
                    throw new KeyNotFoundException($"Product with ID {id} not found.");
                }

                _unitOfWork.ProductsRepository.Delete(product);
                await _unitOfWork.SaveChangesAsync();

                // Invalidate cache
                _memoryCache.Remove(CacheMemory.Product);
                _memoryCache.Remove($"{CacheMemory.Product}_{id}");

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

        //-------------------------------------------------------------------------------------------------------------------------------------------
        public async Task AddProductsFromExcelAsync(IFormFile excelFile)
        {
            if (excelFile == null || excelFile.Length <= 0)
            {
                throw new ArgumentException("Invalid file.");
            }

            // Ensure the file is an Excel file
            if (!excelFile.FileName.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException("Invalid file format. Please upload an Excel file.");
            }

            try
            {
                using (var stream = new MemoryStream())
                {
                    await excelFile.CopyToAsync(stream);
                    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                    using (var package = new ExcelPackage(stream))
                    {
                        var worksheet = package.Workbook.Worksheets.FirstOrDefault();
                        if (worksheet == null)
                        {
                            throw new ArgumentException("The Excel file does not contain any worksheets.");
                        }

                        var columnMapping = new Dictionary<string, int>();
                        for (int col = 1; col <= worksheet.Dimension.End.Column; col++)
                        {
                            var columnName = worksheet.Cells[1, col].Text;
                            if (!string.IsNullOrEmpty(columnName))
                            {
                                columnMapping[columnName] = col;
                            }
                        }

                        for (int row = 2; row <= worksheet.Dimension.End.Row; row++) // Skip header row
                        {
                            var productDto = new ProductDto
                            {
                                Name = GetValue(worksheet, row, columnMapping, "Name"),
                                Description = GetValue(worksheet, row, columnMapping, "Description"),
                                Price = decimal.TryParse(GetValue(worksheet, row, columnMapping, "Price"), out var price) ? price : 0,
                                Processors = GetListValue(worksheet, row, columnMapping, "Processors"),
                                RAM = GetListValue(worksheet, row, columnMapping, "RAM"),
                                Storage = GetListValue(worksheet, row, columnMapping, "Storage"),
                                GraphicsCards = GetListValue(worksheet, row, columnMapping, "GraphicsCards"),
                                ScreenSizes = GetListValue(worksheet, row, columnMapping, "ScreenSizes"),
                                BatteryLives = GetListValue(worksheet, row, columnMapping, "BatteryLives"),
                                OperatingSystems = GetListValue(worksheet, row, columnMapping, "OperatingSystems"),
                                MacModel = GetValue(worksheet, row, columnMapping, "MacModel"),
                                DisplayResolution = GetValue(worksheet, row, columnMapping, "DisplayResolution"),
                                Ports = GetListValue(worksheet, row, columnMapping, "Ports"),
                                Webcam = GetValue(worksheet, row, columnMapping, "Webcam"),
                                Weight = GetValue(worksheet, row, columnMapping, "Weight"),
                                Color = GetValue(worksheet, row, columnMapping, "Color"),
                                Connectivity = GetValue(worksheet, row, columnMapping, "Connectivity"),
                                KeyboardType = GetValue(worksheet, row, columnMapping, "KeyboardType"),
                                TouchBar = GetValue(worksheet, row, columnMapping, "TouchBar"),
                                Popular = bool.TryParse(GetValue(worksheet, row, columnMapping, "Popular"), out var popular) && popular
                            };

                            var categoryName = GetValue(worksheet, row, columnMapping, "CategoryName");
                            var category = await _categoryService.GetCategoryByNameAsync(categoryName);
                            if (category != null)
                            {
                                productDto.CategoryId = category.Id;
                            }
                            else
                            {
                                Console.WriteLine($"Category '{categoryName}' not found. Skipping product '{productDto.Name}'.");
                                continue;
                            }

                            await CreateProductAsync(productDto);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing Excel file: {ex.Message}");
                throw;
            }
        }

        private static string GetValue(ExcelWorksheet worksheet, int row, Dictionary<string, int> columnMapping, string columnName)
        {
            return columnMapping.TryGetValue(columnName, out var col) ? worksheet.Cells[row, col].Text : string.Empty;
        }

        private static List<string> GetListValue(ExcelWorksheet worksheet, int row, Dictionary<string, int> columnMapping, string columnName)
        {
            var value = GetValue(worksheet, row, columnMapping, columnName);
            return string.IsNullOrEmpty(value) ? new List<string>() : value.Split(',').Select(s => s.Trim()).ToList();
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------
        public async Task<IEnumerable<ProductDto>> GetLaptopProductsAsync()
        {
            try
            {
                // Check if the products are cached
                if (!_memoryCache.TryGetValue(CacheMemory.Product, out IEnumerable<ProductDto> cachedProducts))
                {
                    // Fetch all products if not cached
                    var products = await GetAllProductsAsync(); // Get all products using your existing method
                    cachedProducts = products.ToList(); // Cache the products
                    _memoryCache.Set(CacheMemory.Product, cachedProducts, TimeSpan.FromMinutes(30));
                }

                // Filter products by laptop-related categories
                var laptopProducts = cachedProducts.Where(p => new[] { "MacBook", "Mac mini", "Mac Pro", "Mac Studio" }.Contains(p.Category.Name) && p.Category.Status && p.Status);

                return laptopProducts;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while fetching laptop products.", ex);
            }
        }

        public async Task<IEnumerable<ProductDto>> GetDesktopProductsAsync()
        {
            try
            {
                // Check if the products are cached
                if (!_memoryCache.TryGetValue(CacheMemory.Product, out IEnumerable<ProductDto> cachedProducts))
                {
                    // Fetch all products if not cached
                    var products = await GetAllProductsAsync(); // Get all products using your existing method
                    cachedProducts = products.ToList(); // Cache the products
                    _memoryCache.Set(CacheMemory.Product, cachedProducts, TimeSpan.FromMinutes(30));
                }

                // Filter products by desktop-related categories
                var desktopProducts = cachedProducts.Where(p => new[] { "iMac", "Mac Pro", "Mac Studio" }.Contains(p.Category.Name) && p.Category.Status && p.Status);

                return desktopProducts;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while fetching desktop products.", ex);
            }
        }

        public async Task<IEnumerable<ProductDto>> GetAccessoryProductsAsync()
        {
            try
            {
                // Check if the products are cached
                if (!_memoryCache.TryGetValue(CacheMemory.Product, out IEnumerable<ProductDto> cachedProducts))
                {
                    // Fetch all products if not cached
                    var products = await GetAllProductsAsync(); // Get all products using your existing method
                    cachedProducts = products.ToList(); // Cache the products
                    _memoryCache.Set(CacheMemory.Product, cachedProducts, TimeSpan.FromMinutes(30));
                }

                // Filter products by accessory-related categories
                var accessoryProducts = cachedProducts.Where(p => new[] { "Accessories", "SequoiaSpare", "Parts" }.Contains(p.Category.Name) && p.Category.Status && p.Status);

                return accessoryProducts;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while fetching accessory products.", ex);
            }
        }
    }
}