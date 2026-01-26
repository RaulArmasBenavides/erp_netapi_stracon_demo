using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupplierServiceNet.CrossCutting.Options
{
    public sealed class CloudinaryOptions
    {
        public string CloudName { get; init; } = default!;
        public string ApiKey { get; init; } = default!;
        public string ApiSecret { get; init; } = default!;
        public string DefaultFolder { get; init; } = "stracon";
        public string SuppliersFolder { get; init; } = "suppliers";  
        public string? LogoPublicId { get; init; }
 
    }
}
