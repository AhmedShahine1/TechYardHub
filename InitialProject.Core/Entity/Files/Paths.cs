using System.Diagnostics;

namespace TechYardHub.Core.Entity.Files
{
    [DebuggerDisplay("{Name,nq}")]
    public class Paths : BaseEntity
    {
        public string Description { get; set; }
        public ICollection<Images>? Images { get; set; }
    }
}