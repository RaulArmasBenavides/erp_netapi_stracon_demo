using PaymentServiceNet.Core.Entities;
using PaymentServiceNet.Core.IRepositorio;
using PaymentServiceNet.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace PaymentServiceNet.Infrastructure.Repository
{
    public sealed class PurchaseRequestRepository : IPurchaseRequestRepository
    {
        private readonly ApplicationDbContext _db;

        public PurchaseRequestRepository(ApplicationDbContext db) => _db = db;

        public Task<PurchaseRequest?> GetByIdAsync(Guid id, CancellationToken ct = default)
            => _db.PurchaseRequests
                  .FirstOrDefaultAsync(x => x.Id == id, ct);

        public async Task<IReadOnlyList<PurchaseRequest>> ListAsync(CancellationToken ct = default)
            => await _db.PurchaseRequests
                        .AsNoTracking()
                        .ToListAsync(ct);

        public async Task<IReadOnlyList<PurchaseRequest>> ListBySupplierAsync(Guid supplierId, CancellationToken ct = default)
            => await _db.PurchaseRequests
                        .AsNoTracking()
                        .Where(x => x.SupplierId == supplierId)
                        .ToListAsync(ct);

        public Task AddAsync(PurchaseRequest request, CancellationToken ct = default)
            => _db.PurchaseRequests.AddAsync(request, ct).AsTask();

        public void Update(PurchaseRequest request)
            => _db.PurchaseRequests.Update(request);

        public void Remove(PurchaseRequest request)
            => _db.PurchaseRequests.Remove(request);
    }
}
