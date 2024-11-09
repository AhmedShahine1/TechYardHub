using AutoMapper;
using TechYardHub.BusinessLayer.Interfaces;
using TechYardHub.Core.DTO.AuthViewModel.CategoryModel;
using TechYardHub.Core.Entity.Files;
using TechYardHub.RepositoryLayer.Interfaces;
using Microsoft.EntityFrameworkCore;
using TechYardHub.Core.Entity.CategoryData;

namespace TechYardHub.BusinessLayer.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileHandling _fileHandling;
        private readonly IMapper _mapper;
        private readonly Paths Categorypath;

        public CategoryService(IUnitOfWork unitOfWork, IFileHandling fileHandling, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _fileHandling = fileHandling;
            _mapper = mapper;
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
            try
            {
                // Fetch categories with image included
                var categories = await _unitOfWork.CategoriesRepository.GetAllAsync(
                    include: query => query.Include(c => c.image));

                if (categories == null || !categories.Any())
                {
                    // Return an empty list or handle as needed
                    return new List<CategoryDto>();
                }

                // Map categories to CategoryDto including image URLs
                var categoryDtos = new List<CategoryDto>();
                foreach (var category in categories)
                {
                    var categoryDto = await MapCategoryToDtoAsync(category);
                    categoryDtos.Add(categoryDto);
                }

                return categoryDtos;
            }
            catch (Exception ex)
            {
                // Log the error and rethrow or handle it gracefully
                throw new Exception("An error occurred while fetching categories.", ex);
            }
        }

        public async Task<CategoryDto> GetCategoryByIdAsync(string id)
        {
            try
            {
                // Fetch category with image included
                var category = await _unitOfWork.CategoriesRepository.FindAsync(a=>a.Id==id,
                    include: query => query.Include(c => c.image));

                if (category == null)
                {
                    // Return null or throw an exception if the category is not found
                    throw new KeyNotFoundException($"Category with ID {id} not found.");
                }

                // Map category to CategoryDto including image URL
                return await MapCategoryToDtoAsync(category);
            }
            catch (Exception ex)
            {
                // Log the error and rethrow or handle it gracefully
                throw new Exception($"An error occurred while fetching category with ID {id}.", ex);
            }
        }

        public async Task<CategoryDto> CreateCategoryAsync(CategoryDto categoryDto)
        {
            try
            {
                var category = _mapper.Map<Category>(categoryDto);
                string imageid;
                // Upload image if provided
                if (categoryDto.Image != null)
                {
                    imageid = await _fileHandling.UploadFile(categoryDto.Image, Categorypath);
                    category.image = await _unitOfWork.ImagesRepository.GetByIdAsync(imageid);
                }

                await _unitOfWork.CategoriesRepository.AddAsync(category);
                await _unitOfWork.SaveChangesAsync();
                return _mapper.Map<CategoryDto>(category);
            }
            catch (Exception ex)
            {
                // Log the error and rethrow or handle it gracefully
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
                    // Handle case where category does not exist
                    throw new KeyNotFoundException($"Category with ID {categoryDto.Id} not found.");
                }

                _mapper.Map(categoryDto, category);

                // Optionally, update image if provided
                if (categoryDto.Image != null)
                {
                    await _fileHandling.UploadFile(categoryDto.Image, Categorypath);
                }

                _unitOfWork.CategoriesRepository.Update(category);
                await _unitOfWork.SaveChangesAsync();
                return _mapper.Map<CategoryDto>(category);
            }
            catch (Exception ex)
            {
                // Log the error and rethrow or handle it gracefully
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
                    // Handle case where category does not exist
                    throw new KeyNotFoundException($"Category with ID {id} not found.");
                }

                // Optionally delete the image file
                if (category.image != null)
                {
                    await _fileHandling.DeleteFile(category.image.Id);
                }

                _unitOfWork.CategoriesRepository.Delete(category);
                await _unitOfWork.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                // Log the error and rethrow or handle it gracefully
                throw new Exception("An error occurred while deleting the category.", ex);
            }
        }
    }
}
