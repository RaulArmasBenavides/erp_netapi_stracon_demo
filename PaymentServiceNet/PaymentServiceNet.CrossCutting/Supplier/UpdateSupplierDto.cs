using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentServiceNet.CrossCutting.Supplier
{
    public sealed class UpdateSupplierDto
    {
        // En PATCH suelen ser opcionales: si vienen null, no se cambian
        public string? Name { get; set; }
        public string? Address { get; set; }
        public string? Phone { get; set; }

        [EmailAddress]
        public string? Email { get; set; }
    }
}
