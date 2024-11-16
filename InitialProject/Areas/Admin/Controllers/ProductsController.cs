using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechYardHub.BusinessLayer.Interfaces;
using TechYardHub.Core.DTO.AuthViewModel.ProductModel;

namespace TechYardHub.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Policy = "Admin")]
    public class ProductsController : Controller
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        // GET: Products
        public async Task<IActionResult> Index()
        {
            var products = await _productService.GetAllProductsAsync();
            return View(products);
        }

        // GET: Products/Details/{id}
        public async Task<IActionResult> Details(string id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // GET: Products/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.Categories = await _productService.GetAllCategoriesAsync();
            return View(new ProductDto());
        }

        // POST: Products/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductDto productDto, List<IFormFile> images)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, "Please correct the errors and try again.");
                ViewBag.Categories = await _productService.GetAllCategoriesAsync();
                return View(productDto);
            }

            // Validate images (optional: check type and size)
            foreach (var image in images)
            {
                if (!image.ContentType.StartsWith("image/"))
                {
                    ModelState.AddModelError("Images", "Only image files are allowed.");
                    return View(productDto);
                }
                if (image.Length > 2 * 1024 * 1024) // 2MB size limit
                {
                    ModelState.AddModelError("Images", "Each image file must be less than 2MB.");
                    return View(productDto);
                }
            }

            productDto.Images = images;
            try
            {
                await _productService.CreateProductAsync(productDto);
                TempData["SuccessMessage"] = "Product created successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "An error occurred while creating the product.");
                Console.WriteLine(ex); // Logging for debugging
                return View(productDto);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadExcelFile(IFormFile excelFile)
        {
            if (excelFile == null || excelFile.Length == 0)
            {
                // Handle case where no file is selected
                ModelState.AddModelError("", "Please select an Excel file.");
                return RedirectToAction(nameof(Create));
            }
            await _productService.AddProductsFromExcelAsync(excelFile);
            return RedirectToAction(nameof(Index));
        }

        // GET: Products/Edit/{id}
        public async Task<IActionResult> Edit(string id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            ViewBag.Categories = await _productService.GetAllCategoriesAsync();
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // POST: Products/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, ProductDto productDto, List<IFormFile> images)
        {
            if (id != productDto.Id)
            {
                return BadRequest("Product ID mismatch.");
            }

            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, "Please correct the errors and try again.");
                ViewBag.Categories = await _productService.GetAllCategoriesAsync();
                return View(productDto);
            }

            // Validate images
            foreach (var image in images)
            {
                if (!image.ContentType.StartsWith("image/"))
                {
                    ModelState.AddModelError("Images", "Only image files are allowed.");
                    return View(productDto);
                }
                if (image.Length > 2 * 1024 * 1024) // 2MB size limit
                {
                    ModelState.AddModelError("Images", "Each image file must be less than 2MB.");
                    return View(productDto);
                }
            }

            productDto.Images = images;
            try
            {
                await _productService.UpdateProductAsync(productDto);
                TempData["SuccessMessage"] = "Product updated successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "An error occurred while updating the product.");
                Console.WriteLine(ex); // Logging for debugging
                return View(productDto);
            }
        }

        [HttpPost, ActionName("UpdateStatus")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatusProductAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest("Invalid product ID.");
            }

            try
            {
                var updatedProduct = await _productService.UpdateStatusProductAsync(id);
                if (updatedProduct == null)
                {
                    return NotFound("Product not found.");
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
        // POST: Products/Delete/{id}
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var success = await _productService.DeleteProductAsync(id);
            if (!success)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
