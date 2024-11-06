using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechYardHub.Core.Entity.ApplicationData
{
    [DebuggerDisplay("{Name,nq}")]
    public class ApplicationRole : IdentityRole
    {
        public string Description { get; set; }
        public string ArName { get; set; }
    }
}
