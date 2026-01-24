using PaymentServiceNet.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentServiceNet.Core.IRepositorio
{
    public interface ISupplierRepository
    {
        Task<Supplier?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<Supplier?> GetByEmailAsync(string email, CancellationToken ct = default);

        Task<IReadOnlyList<Supplier>> ListAsync(CancellationToken ct = default);

        Task AddAsync(Supplier supplier, CancellationToken ct = default);
        void Update(Supplier supplier);
        void Remove(Supplier supplier);
    }
}
