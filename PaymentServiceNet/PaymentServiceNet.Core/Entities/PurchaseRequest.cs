using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentServiceNet.Core.Entities
{
    public enum PurchaseRequestStatus
    {
        Draft = 0,
        Submitted = 1,
        Approved = 2,
        Rejected = 3,
        Ordered = 4
    }

    public sealed class PurchaseRequest
    {
        public Guid Id { get; private set; }
        public Guid SupplierId { get; private set; }
        public Guid RequestedByUserId { get; private set; }
        public string Description { get; private set; }
        public PurchaseRequestStatus Status { get; private set; }
        public DateTimeOffset CreatedAt { get; private set; }

        private PurchaseRequest() { } // Para ORM

        public PurchaseRequest(
            Guid id,
            Guid supplierId,
            Guid requestedByUserId,
            string description,
            PurchaseRequestStatus status = PurchaseRequestStatus.Draft,
            DateTimeOffset? createdAt = null)
        {
            Id = id == Guid.Empty ? Guid.NewGuid() : id;
            SupplierId = supplierId != Guid.Empty ? supplierId : throw new ArgumentException("SupplierId is required.", nameof(supplierId));
            RequestedByUserId = requestedByUserId != Guid.Empty ? requestedByUserId : throw new ArgumentException("RequestedByUserId is required.", nameof(requestedByUserId));

            Description = Guard.NotNullOrWhiteSpace(description, nameof(description));
            Status = status;
            CreatedAt = createdAt ?? DateTimeOffset.UtcNow;
        }

        public void Submit() => Status = PurchaseRequestStatus.Submitted;
        public void Approve() => Status = PurchaseRequestStatus.Approved;
        public void Reject() => Status = PurchaseRequestStatus.Rejected;
        public void MarkAsOrdered() => Status = PurchaseRequestStatus.Ordered;

        public void UpdateDescription(string description)
        {
            Description = Guard.NotNullOrWhiteSpace(description, nameof(description));
        }
    }
}
