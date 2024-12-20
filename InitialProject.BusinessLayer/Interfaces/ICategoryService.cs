﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechYardHub.Core.DTO.AuthViewModel.CategoryModel;

namespace TechYardHub.BusinessLayer.Interfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync();
        Task<CategoryDto> GetCategoryByIdAsync(string id);
        Task<CategoryDto> GetCategoryByNameAsync(string name);
        Task<CategoryDto> CreateCategoryAsync(CategoryDto categoryDto);
        Task<CategoryDto> UpdateCategoryAsync(CategoryDto categoryDto);
        Task<CategoryDto> UpdateStatusCategoryAsync(string id);
        Task<bool> DeleteCategoryAsync(string id);
    }
}
