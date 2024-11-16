using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;

namespace TechYardHub.Core.DTO.AuthViewModel.CategoryModel
{
    public class CategoryDto
    {
        public string? Id { get; set; } = Guid.NewGuid().ToString();

        [Required(ErrorMessage = "Category Name is required.")]
        [StringLength(100, ErrorMessage = "Category Name cannot exceed 100 characters.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        [StringLength(250, ErrorMessage = "Description cannot exceed 250 characters.")]
        public string Description { get; set; }

        public string? ImageUrl { get; set; }

        [Display(Name = "Category Image")]
        public IFormFile? Image { get; set; }

        public bool Status { get; set; } = true;
    }
}
