using AutoMapper;
using TechYardHub.BusinessLayer.Interfaces;
using TechYardHub.Core.DTO.AuthViewModel.CategoryModel;
using TechYardHub.Core.Entity.Files;
using TechYardHub.RepositoryLayer.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using TechYardHub.Core.Entity.CategoryData;
using TechYardHub.Core.Helpers;

namespace TechYardHub.BusinessLayer.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileHandling _fileHandling;
        private readonly IMapper _mapper;
        private readonly IMemoryCache _memoryCache;
        private readonly Paths Categorypath;

        public CategoryService(IUnitOfWork unitOfWork, IFileHandling fileHandling, IMapper mapper, IMemoryCache memoryCache)
        {
            _unitOfWork = unitOfWork;
            _fileHandling = fileHandling;
            _mapper = mapper;
            _memoryCache = memoryCache;
            Categorypath = unitOfWork.PathsRepository.Find(a => a.Name == "CategoryIcon");
        }

        // Helper method to map Category to CategoryDto with image URL
        private async Task<CategoryDto> MapCategoryToDtoAsync(Category category)
        {
            var categoryDto = _mapper.Map<CategoryDto>(category);

            if (category.image != null)
            {
                categoryDto.ImageUrl = await _fileHandling.GetFile(category.image.Id);
            }

            return categoryDto;
        }

        public async Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync()
        {
            // Use memory cache to fetch categories
            if (!_memoryCache.TryGetValue(CacheMemory.Category.ToString(), out List<CategoryDto> cachedCategories))
            {
                // If not found in cache, fetch from the database
                var categories = await _unitOfWork.CategoriesRepository.GetAllAsync(
                    include: query => query.Include(c => c.image));

                if (categories == null || !categories.Any())
                {
                    return new List<CategoryDto>();
                }

                cachedCategories = new List<CategoryDto>();
                foreach (var category in categories)
                {
                    var categoryDto = await MapCategoryToDtoAsync(category);
                    cachedCategories.Add(categoryDto);
                }

                // Store categories in memory cache
                _memoryCache.Set(CacheMemory.Category.ToString(), cachedCategories, TimeSpan.FromHours(1));
            }

            return cachedCategories;
        }

        public async Task<CategoryDto> GetCategoryByIdAsync(string id)
        {
            var categories = await GetAllCategoriesAsync();
            return categories.FirstOrDefault(c => c.Id == id);
        }

        public async Task<CategoryDto> GetCategoryByNameAsync(string name)
        {
            var categories = await GetAllCategoriesAsync();
            return categories.FirstOrDefault(c => c.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        public async Task<CategoryDto> CreateCategoryAsync(CategoryDto categoryDto)
        {
            try
            {
                var category = _mapper.Map<Category>(categoryDto);
                string imageid;

                if (categoryDto.Image != null)
                {
                    imageid = await _fileHandling.UploadFile(categoryDto.Image, Categorypath);
                    category.image = await _unitOfWork.ImagesRepository.GetByIdAsync(imageid);
                }

                await _unitOfWork.CategoriesRepository.AddAsync(category);
                await _unitOfWork.SaveChangesAsync();

                // Clear cache after creating a new category
                _memoryCache.Remove(CacheMemory.Category.ToString());

                return _mapper.Map<CategoryDto>(category);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while creating the category.", ex);
            }
        }

        public async Task<CategoryDto> UpdateCategoryAsync(CategoryDto categoryDto)
        {
            try
            {
                var category = await _unitOfWork.CategoriesRepository.GetByIdAsync(categoryDto.Id);
                if (category == null)
                {
                    throw new KeyNotFoundException($"Category with ID {categoryDto.Id} not found.");
                }

                _mapper.Map(categoryDto, category);

                if (categoryDto.Image != null)
                {
                    await _fileHandling.UploadFile(categoryDto.Image, Categorypath);
                }

                _unitOfWork.CategoriesRepository.Update(category);
                await _unitOfWork.SaveChangesAsync();

                // Clear cache after updating a category
                _memoryCache.Remove(CacheMemory.Category.ToString());

                return _mapper.Map<CategoryDto>(category);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while updating the category.", ex);
            }
        }

        public async Task<CategoryDto> UpdateStatusCategoryAsync(string id)
        {
            try
            {
                var category = await _unitOfWork.CategoriesRepository.GetByIdAsync(id);
                if (category == null)
                {
                    throw new KeyNotFoundException($"Category with ID {id} not found.");
                }

                category.Status = !category.Status;
                _unitOfWork.CategoriesRepository.Update(category);
                await _unitOfWork.SaveChangesAsync();

                // Clear cache after updating category status
                _memoryCache.Remove(CacheMemory.Category.ToString());

                return _mapper.Map<CategoryDto>(category);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while updating the category.", ex);
            }
        }

        public async Task<bool> DeleteCategoryAsync(string id)
        {
            try
            {
                var category = await _unitOfWork.CategoriesRepository.GetByIdAsync(id);
                if (category == null)
                {
                    throw new KeyNotFoundException($"Category with ID {id} not found.");
                }

                if (category.image != null)
                {
                    await _fileHandling.DeleteFile(category.image.Id);
                }

                _unitOfWork.CategoriesRepository.Delete(category);
                await _unitOfWork.SaveChangesAsync();

                // Clear cache after deleting a category
                _memoryCache.Remove(CacheMemory.Category.ToString());

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while deleting the category.", ex);
            }
        }
    }
}
