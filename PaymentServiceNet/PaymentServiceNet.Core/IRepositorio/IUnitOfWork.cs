using PaymentServiceNet.Core.IRepositorio;

namespace PaymentServiceNet.Core.IRepositorio
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
