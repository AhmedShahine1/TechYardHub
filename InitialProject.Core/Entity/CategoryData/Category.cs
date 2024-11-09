using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechYardHub.Core.Entity.Files;
using TechYardHub.Core.Entity.ProductData;

namespace TechYardHub.Core.Entity.CategoryData
{
    [DebuggerDisplay("{name,nq}")]
    public class Category
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string name { get; set; }
        public string description { get; set; }
        public Images image { get; set; }
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}