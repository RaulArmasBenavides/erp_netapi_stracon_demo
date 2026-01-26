using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupplierServiceNet.CrossCutting.Supplier
{
    public sealed class CreateSupplierDto
    {
        [Required, MinLength(2)]
        public string Name { get; set; } = default!;

        [Required, MinLength(2)]
        public string Address { get; set; } = default!;

        [Required, MinLength(6)]
        public string Phone { get; set; } = default!;

        [Required, EmailAddress]
        public string Email { get; set; } = default!;

        public IFormFile? Photo { get; set; } // <- nueva propiedad
    }
}
