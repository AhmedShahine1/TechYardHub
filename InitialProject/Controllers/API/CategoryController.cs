using Microsoft.AspNetCore.Mvc;
using TechYardHub.BusinessLayer.Interfaces;
using TechYardHub.Core.DTO;
using TechYardHub.Core.DTO.AuthViewModel.CategoryModel;

namespace TechYardHub.Controllers.API
{
    public class CategoryController : BaseController
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        public async Task<ActionResult<BaseResponse>> GetAllCategories()
        {
            try
            {
                var categories = await _categoryService.GetAllCategoriesAsync();
                return Ok(new BaseResponse
                {
                    Data = categories,
                    ErrorCode = 200,
                    ErrorMessage = null,
                    status = true
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new BaseResponse
                {
                    status = false,
                    ErrorCode = 500,
                    ErrorMessage = ex.Message
                });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BaseResponse>> GetCategoryById(string id)
        {
            try
            {
                var category = await _categoryService.GetCategoryByIdAsync(id);
                if (category == null)
                {
                    return NotFound(new BaseResponse
                    {
                        status = false,
                        ErrorCode = 404,
                        ErrorMessage = "Category not found"
                    });
                }

                return Ok(new BaseResponse
                {
                    Data = category,
                    ErrorCode = 200,
                    status = true
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new BaseResponse
                {
                    status = false,
                    ErrorCode = 500,
                    ErrorMessage = ex.Message
                });
            }
        }
    }
}
