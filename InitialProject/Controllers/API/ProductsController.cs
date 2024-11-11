using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TechYardHub.BusinessLayer.Interfaces;
using TechYardHub.Core.DTO;
using TechYardHub.Core.DTO.AuthViewModel.ProductModel;

namespace TechYardHub.Controllers.API
{
    public class ProductsController : BaseController
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        // GET: api/products
        [HttpGet]
        public async Task<ActionResult<BaseResponse>> GetAllProducts()
        {
            try
            {
                var products = await _productService.GetAllProductsAsync();
                var response = new BaseResponse
                {
                    status = true,
                    ErrorCode = 200,
                    ErrorMessage = null,
                    Data = products
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new BaseResponse
                {
                    status = false,
                    ErrorCode = 500,
                    ErrorMessage = ex.Message,
                    Data = null
                });
            }
        }

        // GET: api/products/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<BaseResponse>> GetProductById(string id)
        {
            try
            {
                var product = await _productService.GetProductByIdAsync(id);
                if (product == null)
                {
                    return NotFound(new BaseResponse
                    {
                        status = false,
                        ErrorCode = 404,
                        ErrorMessage = "Product not found.",
                        Data = null
                    });
                }

                var response = new BaseResponse
                {
                    status = true,
                    ErrorCode = 200,
                    ErrorMessage = null,
                    Data = product
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new BaseResponse
                {
                    status = false,
                    ErrorCode = 500,
                    ErrorMessage = ex.Message,
                    Data = null
                });
            }
        }

    }
}
