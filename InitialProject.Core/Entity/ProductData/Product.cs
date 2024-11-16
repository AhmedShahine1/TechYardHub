using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using TechYardHub.Core.Entity.CategoryData;
using TechYardHub.Core.Entity.Files;

namespace TechYardHub.Core.Entity.ProductData
{
    [DebuggerDisplay("{Name,nq}")]
    public class Product : BaseEntity
    {
        public string Description { get; set; }
        public decimal Price { get; set; }
        public bool Popular { get; set; } = false;
        // Foreign key to the Category
        public string CategoryId { get; set; }
        [ForeignKey(nameof(CategoryId))]
        public Category Category { get; set; }

        // Collection for multiple images
        public ICollection<Images> Images { get; set; } = new List<Images>();

        // Apple-specific properties for different types of Macs
        public List<string> Processors { get; set; } = new List<string>();
        public List<string> RAM { get; set; } = new List<string>();
        public List<string> Storage { get; set; } = new List<string>();
        public List<string> GraphicsCards { get; set; } = new List<string>();
        public List<string> ScreenSizes { get; set; } = new List<string>();
        public List<string> BatteryLives { get; set; } = new List<string>();
        public List<string> OperatingSystems { get; set; } = new List<string>();

        // Specific properties for different Mac models
        public string? MacModel { get; set; } // "MacBook", "iMac", "Mac Mini", etc.
        public string? DisplayResolution { get; set; }
        public List<string> Ports { get; set; } = new List<string>();
        public string? Webcam { get; set; }
        public string? Weight { get; set; }
        public string? Color { get; set; }
        public string? Connectivity { get; set; }

        // Additional for specific models
        public string? KeyboardType { get; set; } // For MacBooks, Mac Minis (e.g. 'Magic Keyboard')
        public string? TouchBar { get; set; } // For MacBook Pro models with Touch Bar
    }
}
