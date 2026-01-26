using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupplierServiceNet.CrossCutting
{
    public sealed class PagedResult<T>
    {
        public List<T> Items { get; set; } = new();
        public int TotalRows { get; set; }

      
    }
}
