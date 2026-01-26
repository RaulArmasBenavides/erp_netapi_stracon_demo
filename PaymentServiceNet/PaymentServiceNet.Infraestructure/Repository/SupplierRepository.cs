 
using Microsoft.EntityFrameworkCore;
using SupplierServiceNet.Core.Entities;
using SupplierServiceNet.Core.IRepositorio;
using SupplierServiceNet.CrossCutting;
using SupplierServiceNet.Infrastructure.Data;
using SupplierServiceNet.Infrastructure.Repositorio;
using System;

namespace SupplierServiceNet.Repositorio
{
    public class SupplierRepository: Repository<Supplier>, ISupplierRepository
    {
   
        private readonly ApplicationDbContext _db;

        public SupplierRepository(ApplicationDbContext mbd) : base(mbd)
        {
            _db = mbd;
        }

        public Task<Supplier?> GetByIdAsync(Guid id, CancellationToken ct = default)
            => _db.Suppliers
                  .FirstOrDefaultAsync(x => x.Id == id, ct);

        public Task<Supplier?> GetByEmailAsync(string email, CancellationToken ct = default)
            => _db.Suppliers
                  .FirstOrDefaultAsync(x => x.Email == email, ct);

        public async Task<IReadOnlyList<Supplier>> ListAsync(CancellationToken ct = default)
            => await _db.Suppliers
                        .AsNoTracking()
                        .ToListAsync(ct);

        public Task AddAsync(Supplier supplier, CancellationToken ct = default)
            => _db.Suppliers.AddAsync(supplier, ct).AsTask();

        public void Update(Supplier supplier)
            => _db.Suppliers.Update(supplier);

        public void Remove(Supplier supplier)
            => _db.Suppliers.Remove(supplier);
    }
}
