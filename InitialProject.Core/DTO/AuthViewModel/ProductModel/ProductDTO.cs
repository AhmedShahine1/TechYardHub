using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechYardHub.Core.Entity.CategoryData;

namespace TechYardHub.Core.DTO.AuthViewModel.ProductModel
{
    public class ProductDto
    {
        public string? Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }

        // Exposing Category Name (if needed) instead of full Category entity
        public string CategoryId { get; set; }
        public Category? Category { get; set; }

        // Collection for multiple image URLs
        public List<string>? ImageUrls { get; set; }
        public List<IFormFile>? Images { get; set; }

        // Apple-specific properties for different types of Macs
        public List<string>? Processors { get; set; }
        public List<string>? RAM { get; set; }
        public List<string>? Storage { get; set; }
        public List<string>? GraphicsCards { get; set; }
        public List<string>? ScreenSizes { get; set; }
        public List<string>? BatteryLives { get; set; }
        public List<string>? OperatingSystems { get; set; }

        // Specific properties for different Mac models
        public string? MacModel { get; set; }
        public string? DisplayResolution { get; set; }
        public List<string>? Ports { get; set; }
        public string? Webcam { get; set; }
        public string? Weight { get; set; }
        public string? Color { get; set; }
        public string? Connectivity { get; set; }

        // Additional for specific models
        public string? KeyboardType { get; set; }
        public string? TouchBar { get; set; }
    }
}
