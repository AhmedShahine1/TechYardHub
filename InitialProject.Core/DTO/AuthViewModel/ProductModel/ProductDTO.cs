using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TechYardHub.Core.DTO.AuthViewModel.CategoryModel;
using TechYardHub.Core.Entity.CategoryData;

namespace TechYardHub.Core.DTO.AuthViewModel.ProductModel
{
    public class ProductDto
    {
        public string? Id { get; set; } = Guid.NewGuid().ToString();

        [Required(ErrorMessage = "Product Name is required.")]
        [StringLength(100, ErrorMessage = "Product Name cannot exceed 100 characters.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Price is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than zero.")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Please select a category.")]
        public string CategoryId { get; set; }
        public CategoryDto? Category { get; set; }

        public List<string> ImageUrls { get; set; } = new List<string>();

        [Display(Name = "Product Images")]
        public List<IFormFile> Images { get; set; } = new List<IFormFile>();

        public List<string> Processors { get; set; } = new List<string>();
        public List<string> RAM { get; set; } = new List<string>();
        public List<string> Storage { get; set; } = new List<string>();
        public List<string> GraphicsCards { get; set; } = new List<string>();
        public List<string> ScreenSizes { get; set; } = new List<string>();
        public List<string> BatteryLives { get; set; } = new List<string>();
        public List<string> OperatingSystems { get; set; } = new List<string>();

        public string? MacModel { get; set; }
        public string? DisplayResolution { get; set; }
        public List<string> Ports { get; set; } = new List<string>();
        public string? Webcam { get; set; }
        public string? Weight { get; set; }
        public string? Color { get; set; }
        public string? Connectivity { get; set; }
        public string? KeyboardType { get; set; }
        public string? TouchBar { get; set; }
        public bool Status { get; set; } = true;
        public bool Popular { get; set; }
    }
}
