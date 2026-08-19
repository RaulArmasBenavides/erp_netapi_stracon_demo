using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SupplierServiceNet.Core.Entities;
using SupplierServiceNet.Core.Interfaces;
using SupplierServiceNet.Core.IRepositorio;
using SupplierServiceNet.CrossCutting.Options;
using SupplierServiceNet.CrossCutting.Supplier;
using SupplierServiceNet.CrossCutting.Dtos.Excel;
using SupplierServiceNet.CrossCutting.Exceptions;

namespace SupplierServiceNet.Application.Services
{
    public sealed class SupplierService : ISupplierService
    {
        private readonly IUnitOfWork _uow;
        private readonly ICloudinaryService _cloudinaryService;
        private readonly CloudinaryOptions _cloudinaryOptions;
        public SupplierService(IUnitOfWork uow, ICloudinaryService cloudinaryService, IOptions<CloudinaryOptions> cloudinaryOptions)
        {
            _uow = uow;
            _cloudinaryService = cloudinaryService;
            _cloudinaryOptions = cloudinaryOptions.Value;
        }

        public async Task<IReadOnlyList<Supplier>> GetAllAsync(CancellationToken ct = default)
        {
            // Ajusta al método real de tu repo:
            // Ejemplos comunes:
            // - GetAllAsync()
            // - GetAll()
            // - GetSuppliers()
            // Si tu repo es sync, envuélvelo o déjalo sync (no ideal).
            return await _uow.Suppliers.GetAllAsync();
        }

        public async Task<Supplier?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            return await _uow.Suppliers.GetByIdAsync(id);
        }

        public async Task<Supplier> CreateAsync(CreateSupplierDto dto, string createdBy, CancellationToken ct = default)
        {
            string? photoId = null;

            // Si hay una imagen, subirla a Cloudinary
            if (dto.Photo != null)
            {
                // Usar el servicio de Cloudinary
                var uploadResult = await this._cloudinaryService.UploadImageAsync(dto.Photo, this._cloudinaryOptions.SuppliersFolder, ct);
                // Asumiendo que uploadResult tiene una propiedad PublicId
                photoId = uploadResult.Url;
            }

            // Dominio: construyes entidad con constructor
            var supplier = new Supplier(
                id: Guid.NewGuid(),
                name: dto.Name,
                address: dto.Address,
                phone: dto.Phone,
                email: dto.Email,
                photoId: photoId,  // Asignamos el public_id de Cloudinary,
                createdBy: createdBy
            );

            await _uow.Suppliers.AddAsync(supplier);
            await _uow.SaveChangesAsync();

            return supplier;
        }

        public async Task<Supplier?> PatchAsync(Guid id, UpdateSupplierDto dto, CancellationToken ct = default)
        {
            var supplier = await _uow.Suppliers.GetByIdAsync(id);
            if (supplier is null) return null;

            // PATCH parcial: si viene null, mantienes el valor actual
            var name = dto.Name ?? supplier.Name;
            var address = dto.Address ?? supplier.Address;
            var phone = dto.Phone ?? supplier.Phone;
            var email = dto.Email ?? supplier.Email;

            supplier.UpdateInfo(name, address, phone, email);

            // Si tu repo requiere Update explícito, úsalo. Si el DbContext trackea, podría no ser necesario.
            _uow.Suppliers.Update(supplier);

            await _uow.SaveChangesAsync();
            return supplier;
        }

        public async Task<Supplier?> PatchPhotoAsync(Guid id, string? photoId, CancellationToken ct = default)
        {
            var supplier = await _uow.Suppliers.GetByIdAsync(id);
            if (supplier is null) return null;

            // PhotoId puede ser null si quieres "quitar" la foto
            supplier.SetPhoto(photoId);

            _uow.Suppliers.Update(supplier);

            await _uow.SaveChangesAsync();
            return supplier;
        }


        public async Task<Supplier?> ApproveAsync(Guid id, string approvedBy, CancellationToken ct = default)
        {
            var supplier = await this._uow.Suppliers.GetByIdAsync(id, ct);
            if (supplier is null) return null;

            supplier.Approve(approvedBy); // idempotente

            _uow.Suppliers.Update(supplier);
            await _uow.SaveChangesAsync();

            return supplier;
        }
        public async Task<bool> DeleteAsync(Guid id, string deletedBy, CancellationToken ct = default)
        {
            var supplier = await _uow.Suppliers.GetByIdAsync(id);
            if (supplier is null) return false;

            supplier.SoftDelete(deletedBy);
            _uow.Suppliers.Update(supplier);
            await _uow.SaveChangesAsync();

            return true;
        }

        public async Task<bool> RestoreAsync(Guid id, CancellationToken ct = default)
        {
            var supplier = await _uow.Suppliers.IgnoreQueryFilters()
                .FirstOrDefaultAsync(s => s.Id == id, ct);
            if (supplier is null) return false;

            supplier.Restore();
            _uow.Suppliers.Update(supplier);
            await _uow.SaveChangesAsync();

            return true;
        }

        public async Task<BulkImportResultDto> BulkImportAsync(List<SupplierImportDto> suppliers, string importedBy, CancellationToken ct = default)
        {
            var result = new BulkImportResultDto
            {
                TotalRows = suppliers.Count
            };

            var allSuppliers = await _uow.Suppliers.GetAllAsync();

            for (int i = 0; i < suppliers.Count; i++)
            {
                var supplierDto = suppliers[i];
                var rowNumber = i + 2; // Excel row number (header is row 1)

                var validationErrors = supplierDto.GetValidationErrors();
                if (validationErrors.Any())
                {
                    result.FailedImports++;
                    result.Errors.Add(new RowErrorDto
                    {
                        RowNumber = rowNumber,
                        Identifier = supplierDto.Id ?? supplierDto.Name ?? "Unknown",
                        Errors = validationErrors
                    });
                    continue;
                }

                try
                {
                    if (supplierDto.IsUpdate)
                    {
                        // Update existing
                        if (!Guid.TryParse(supplierDto.Id, out var supplierId))
                        {
                            result.FailedImports++;
                            result.Errors.Add(new RowErrorDto
                            {
                                RowNumber = rowNumber,
                                Identifier = supplierDto.Id ?? supplierDto.Name ?? "Unknown",
                                Errors = new List<string> { "Invalid ID format (must be GUID)" }
                            });
                            continue;
                        }

                        var existing = allSuppliers.FirstOrDefault(s => s.Id == supplierId);
                        if (existing == null)
                        {
                            result.FailedImports++;
                            result.Errors.Add(new RowErrorDto
                            {
                                RowNumber = rowNumber,
                                Identifier = supplierDto.Id,
                                Errors = new List<string> { $"Supplier with ID {supplierId} not found" }
                            });
                            continue;
                        }

                        existing.UpdateInfo(supplierDto.Name, supplierDto.Address, supplierDto.Phone, supplierDto.Email);
                        _uow.Suppliers.Update(existing);
                    }
                    else
                    {
                        // Create new
                        var newSupplier = new Supplier(
                            id: Guid.NewGuid(),
                            name: supplierDto.Name!,
                            address: supplierDto.Address,
                            phone: supplierDto.Phone!,
                            email: supplierDto.Email!,
                            photoId: null,
                            createdBy: importedBy
                        );

                        await _uow.Suppliers.AddAsync(newSupplier);
                    }

                    result.SuccessfulImports++;
                }
                catch (Exception ex)
                {
                    result.FailedImports++;
                    result.Errors.Add(new RowErrorDto
                    {
                        RowNumber = rowNumber,
                        Identifier = supplierDto.Id ?? supplierDto.Name ?? "Unknown",
                        Errors = new List<string> { $"Error processing row: {ex.Message}" }
                    });
                }
            }

            // Save all changes at once
            if (result.SuccessfulImports > 0)
            {
                await _uow.SaveChangesAsync();
            }

            result.Message = result.FailedImports == 0
                ? $"Successfully imported {result.SuccessfulImports} suppliers"
                : $"Imported {result.SuccessfulImports} of {result.TotalRows} suppliers. {result.FailedImports} failed.";

            return result;
        }

        public async Task<List<SupplierExportDto>> GetForExportAsync(CancellationToken ct = default)
        {
            var suppliers = await _uow.Suppliers.GetAllAsync();

            return suppliers.Select(s => new SupplierExportDto
            {
                Id = s.Id.ToString(),
                Name = s.Name,
                Email = s.Email,
                Phone = s.Phone,
                Address = s.Address,
                Status = s.IsApproved ? "Approved" : "Pending",
                CreatedBy = s.CreatedBy,
                CreatedAt = s.CreatedAt.DateTime
            }).ToList();
        }
    }
}
