using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentServiceNet.Core.Entities
{
    public sealed class Supplier
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public string Address { get; private set; }
        public string Phone { get; private set; }
        public string Email { get; private set; }
        public string? PhotoId { get; private set; } // Cloudinary public_id o GUID
        public DateTimeOffset CreatedAt { get; private set; }

        // Navegación de dominio (opcional). En DDD suele ser agregado raíz.
        private readonly List<PurchaseRequest> _purchaseRequests = new();
        public IReadOnlyCollection<PurchaseRequest> PurchaseRequests => _purchaseRequests.AsReadOnly();

        private Supplier() { } // Para ORM si luego lo necesitas

        public Supplier(
            Guid id,
            string name,
            string address,
            string phone,
            string email,
            string? photoId = null,
            DateTimeOffset? createdAt = null)
        {
            Id = id == Guid.Empty ? Guid.NewGuid() : id;

            Name = Guard.NotNullOrWhiteSpace(name, nameof(name));
            Address = Guard.NotNullOrWhiteSpace(address, nameof(address));
            Phone = Guard.NotNullOrWhiteSpace(phone, nameof(phone));
            Email = Guard.NotNullOrWhiteSpace(email, nameof(email));

            PhotoId = photoId;
            CreatedAt = createdAt ?? DateTimeOffset.UtcNow;
        }

        public void UpdateInfo(string name, string address, string phone, string email)
        {
            Name = Guard.NotNullOrWhiteSpace(name, nameof(name));
            Address = Guard.NotNullOrWhiteSpace(address, nameof(address));
            Phone = Guard.NotNullOrWhiteSpace(phone, nameof(phone));
            Email = Guard.NotNullOrWhiteSpace(email, nameof(email));
        }

        public void SetPhoto(string? photoId)
        {
            PhotoId = photoId;
        }

        public PurchaseRequest CreatePurchaseRequest(Guid requestedByUserId, string description)
        {
            var pr = new PurchaseRequest(
                id: Guid.NewGuid(),
                supplierId: Id,
                requestedByUserId: requestedByUserId,
                description: description
            );

            _purchaseRequests.Add(pr);
            return pr;
        }
    }

    internal static class Guard
    {
        public static string NotNullOrWhiteSpace(string value, string paramName)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Value cannot be null or whitespace.", paramName);
            return value.Trim();
        }
    }
}
