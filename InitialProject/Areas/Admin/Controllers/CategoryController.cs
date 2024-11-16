using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechYardHub.BusinessLayer.Interfaces;
using TechYardHub.Core.DTO.AuthViewModel.CategoryModel;

namespace TechYardHub.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Policy = "Admin")]
    public class CategoryController : Controller
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        public async Task<IActionResult> Index()
        {
            var categories = await _categoryService.GetAllCategoriesAsync();
            return View(categories);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryDto categoryDto)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, "Please correct the errors and try again.");
                return View(categoryDto);
            }

            try
            {
                // Check if the category name is unique (optional)
                var existingCategory = await _categoryService.GetCategoryByNameAsync(categoryDto.Name);
                if (existingCategory != null)
                {
                    ModelState.AddModelError("Name", "A category with this name already exists.");
                    return View(categoryDto);
                }

                // Validate image file if provided
                if (categoryDto.Image != null)
                {
                    if (!categoryDto.Image.ContentType.StartsWith("image/"))
                    {
                        ModelState.AddModelError("Image", "Please upload a valid image file.");
                        return View(categoryDto);
                    }
                    if (categoryDto.Image.Length > 2 * 1024 * 1024) // 2MB max size
                    {
                        ModelState.AddModelError("Image", "Image size cannot exceed 2MB.");
                        return View(categoryDto);
                    }
                }

                await _categoryService.CreateCategoryAsync(categoryDto);
                TempData["SuccessMessage"] = "Category created successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "An error occurred while creating the category.");
                // Log the exception for further analysis
                Console.WriteLine(ex); // Replace with your logging service
                return View(categoryDto);
            }
        }

        public async Task<IActionResult> Edit(string id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            if (category == null)
                return NotFound();
            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, CategoryDto categoryDto)
        {
            if (id != categoryDto.Id)
            {
                return BadRequest("Category ID mismatch.");
            }

            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, "Please correct the errors and try again.");
                return View(categoryDto);
            }

            try
            {
                // Check if another category with the same name already exists (optional)
                var existingCategory = await _categoryService.GetCategoryByNameAsync(categoryDto.Name);
                if (existingCategory != null && existingCategory.Id != id)
                {
                    ModelState.AddModelError("Name", "A category with this name already exists.");
                    return View(categoryDto);
                }

                // Validate image file if provided
                if (categoryDto.Image != null)
                {
                    if (!categoryDto.Image.ContentType.StartsWith("image/"))
                    {
                        ModelState.AddModelError("Image", "Please upload a valid image file.");
                        return View(categoryDto);
                    }
                    if (categoryDto.Image.Length > 2 * 1024 * 1024) // 2MB max size
                    {
                        ModelState.AddModelError("Image", "Image size cannot exceed 2MB.");
                        return View(categoryDto);
                    }
                }

                await _categoryService.UpdateCategoryAsync(categoryDto);
                TempData["SuccessMessage"] = "Category updated successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "An error occurred while updating the category.");
                Console.WriteLine(ex); // Replace with logging service
                return View(categoryDto);
            }
        }

        [HttpPost, ActionName("UpdateStatus")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatusCategoryAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest("Invalid category ID.");
            }

            try
            {
                var updatedCategory = await _categoryService.UpdateStatusCategoryAsync(id);
                if (updatedCategory == null)
                {
                    return NotFound("Category not found.");
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            await _categoryService.DeleteCategoryAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
