using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentServiceNet.CrossCutting.Dtos
{
    public sealed class SupplierDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = default!;
        public string Address { get; set; } = default!;
        public string Phone { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string? PhotoId { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
    }
}
