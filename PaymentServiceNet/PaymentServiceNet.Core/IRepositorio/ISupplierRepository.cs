using SupplierServiceNet.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupplierServiceNet.Core.IRepositorio
{
    public interface ISupplierRepository : IRepository<Supplier>
    {
        Task<Supplier?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<Supplier?> GetByEmailAsync(string email, CancellationToken ct = default);

        Task<IReadOnlyList<Supplier>> ListAsync(CancellationToken ct = default);

        Task AddAsync(Supplier supplier, CancellationToken ct = default);
        void Update(Supplier supplier);
        void Remove(Supplier supplier);
    }
}
