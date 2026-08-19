using SupplierServiceNet.Core.Entities;
using SupplierServiceNet.CrossCutting.Supplier;
using SupplierServiceNet.CrossCutting.Dtos.Excel;

namespace SupplierServiceNet.Core.Interfaces
{
    public interface ISupplierService
    {
        Task<IReadOnlyList<Core.Entities.Supplier>> GetAllAsync(CancellationToken ct = default);
        Task<Core.Entities.Supplier?> GetByIdAsync(Guid id, CancellationToken ct = default);

        Task<Core.Entities.Supplier> CreateAsync(CreateSupplierDto dto, string createdBy, CancellationToken ct = default);

        Task<Core.Entities.Supplier?> PatchAsync(Guid id, UpdateSupplierDto dto, CancellationToken ct = default);
        Task<Core.Entities.Supplier?> PatchPhotoAsync(Guid id, string? photoId, CancellationToken ct = default);

        Task<Supplier?> ApproveAsync(Guid id, string approvedBy, CancellationToken ct = default);
        Task<bool> DeleteAsync(Guid id, string deletedBy, CancellationToken ct = default);
        Task<bool> RestoreAsync(Guid id, CancellationToken ct = default);

        Task<BulkImportResultDto> BulkImportAsync(List<SupplierImportDto> suppliers, string importedBy, CancellationToken ct = default);
        Task<List<SupplierExportDto>> GetForExportAsync(CancellationToken ct = default);
    }
}
