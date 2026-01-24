 
using Microsoft.EntityFrameworkCore;
using PaymentServiceNet.Core.Entities;
using PaymentServiceNet.Core.IRepositorio;
using PaymentServiceNet.CrossCutting;
using PaymentServiceNet.Infrastructure.Data;
using System;

namespace PaymentServiceNet.Repositorio
{
    public class SupplierRepository : ISupplierRepository
    {
        private readonly ApplicationDbContext _db;

   
        public SupplierRepository(ApplicationDbContext db) => _db = db;

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
