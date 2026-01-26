using SupplierServiceNet.Core.IRepositorio;

namespace SupplierServiceNet.Core.IRepositorio
{
    public interface IUnitOfWork : IDisposable
    {
        IPurchaseRequestRepository PurchaseRequests { get; }
        ISupplierRepository Suppliers { get; }
        IUserRepository Users { get; }
        void Save();
        Task<int> SaveChangesAsync();
    }
}
