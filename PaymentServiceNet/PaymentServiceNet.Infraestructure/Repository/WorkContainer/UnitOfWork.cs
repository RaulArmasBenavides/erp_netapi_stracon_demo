using SupplierServiceNet.Core.IRepositorio;
using SupplierServiceNet.Infrastructure.Data;
using SupplierServiceNet.Infrastructure.Repository;
using SupplierServiceNet.Repositorio;

namespace SupplierServiceNet.Infrastructure.Repositorio.WorkContainer
{
    public class UnitOfWork : IUnitOfWork
    {

        private readonly ApplicationDbContext db;
        public UnitOfWork( ApplicationDbContext mdb) {
            db = mdb;
            PurchaseRequests = new PurchaseRequestRepository(mdb);
            Suppliers = new SupplierRepository(mdb);
            Users = new UserRepository(mdb, null);
        }
        public IPurchaseRequestRepository PurchaseRequests { get; private set; }
        public ISupplierRepository Suppliers { get; private set; }
        public IUserRepository Users { get; private set; }
        public void Dispose()
        {
            db.Dispose();
        }
        public void Save()
        {
            db.SaveChanges();
        }
        public Task<int> SaveChangesAsync()
        {
            return db.SaveChangesAsync();
        }
    }
}
