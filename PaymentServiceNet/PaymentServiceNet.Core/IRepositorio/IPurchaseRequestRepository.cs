using PaymentServiceNet.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentServiceNet.Core.IRepositorio
{
    public interface IPurchaseRequestRepository
    {
        Task<PurchaseRequest?> GetByIdAsync(Guid id, CancellationToken ct = default);

        Task<IReadOnlyList<PurchaseRequest>> ListAsync(CancellationToken ct = default);
        Task<IReadOnlyList<PurchaseRequest>> ListBySupplierAsync(Guid supplierId, CancellationToken ct = default);

        Task AddAsync(PurchaseRequest request, CancellationToken ct = default);
        void Update(PurchaseRequest request);
        void Remove(PurchaseRequest request);
    }
}
