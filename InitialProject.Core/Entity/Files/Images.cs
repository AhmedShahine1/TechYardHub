using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;

namespace TechYardHub.Core.Entity.Files
{
    [DebuggerDisplay("{Name,nq}")]
    public class Images : BaseEntity
    {
        [ForeignKey("path")]
        public string pathId { get; set; }

        public Paths path { get; set; }
    }
}