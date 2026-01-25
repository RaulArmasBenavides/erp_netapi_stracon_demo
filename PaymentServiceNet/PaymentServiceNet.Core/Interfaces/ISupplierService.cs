using PaymentServiceNet.Core.Entities;
using PaymentServiceNet.CrossCutting.Supplier;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentServiceNet.Core.Interfaces
{
    public interface ISupplierService
    {
        Task<IReadOnlyList<Core.Entities.Supplier>> GetAllAsync(CancellationToken ct = default);
        Task<Core.Entities.Supplier?> GetByIdAsync(Guid id, CancellationToken ct = default);

        Task<Core.Entities.Supplier> CreateAsync(CreateSupplierDto dto, CancellationToken ct = default);

        Task<Core.Entities.Supplier?> PatchAsync(Guid id, UpdateSupplierDto dto, CancellationToken ct = default);
        Task<Core.Entities.Supplier?> PatchPhotoAsync(Guid id, string? photoId, CancellationToken ct = default);

        Task<Supplier?> ApproveAsync(Guid id, string approvedBy, CancellationToken ct = default);
        Task<bool> DeleteAsync(Guid id, CancellationToken ct = default);
    }
}
