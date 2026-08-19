using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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
    public sealed class SupplierServiceWithLogging : ISupplierService
    {
        private readonly IUnitOfWork _uow;
        private readonly ICloudinaryService _cloudinaryService;
        private readonly CloudinaryOptions _cloudinaryOptions;
        private readonly ILogger<SupplierServiceWithLogging> _logger;

        public SupplierServiceWithLogging(
            IUnitOfWork uow,
            ICloudinaryService cloudinaryService,
            IOptions<CloudinaryOptions> cloudinaryOptions,
            ILogger<SupplierServiceWithLogging> logger)
        {
            _uow = uow;
            _cloudinaryService = cloudinaryService;
            _cloudinaryOptions = cloudinaryOptions.Value;
            _logger = logger;
        }

        public async Task<IReadOnlyList<Supplier>> GetAllAsync(CancellationToken ct = default)
        {
            _logger.LogInformation("Fetching all suppliers");
            try
            {
                var suppliers = await _uow.Suppliers.GetAllAsync();
                _logger.LogInformation("Successfully fetched {Count} suppliers", suppliers.Count);
                return suppliers;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching suppliers");
                throw;
            }
        }

        public async Task<Supplier?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            _logger.LogInformation("Fetching supplier with ID: {SupplierId}", id);
            try
            {
                var supplier = await _uow.Suppliers.GetByIdAsync(id);
                if (supplier == null)
                {
                    _logger.LogWarning("Supplier not found: {SupplierId}", id);
                    return null;
                }
                _logger.LogInformation("Successfully fetched supplier: {SupplierId}", id);
                return supplier;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching supplier {SupplierId}", id);
                throw;
            }
        }

        public async Task<Supplier> CreateAsync(CreateSupplierDto dto, string createdBy, CancellationToken ct = default)
        {
            _logger.LogInformation("Creating new supplier: {Name} by {CreatedBy}", dto.Name, createdBy);
            try
            {
                string? photoId = null;

                if (dto.Photo != null)
                {
                    _logger.LogInformation("Uploading photo for supplier: {Name}", dto.Name);
                    var uploadResult = await this._cloudinaryService.UploadImageAsync(
                        dto.Photo,
                        this._cloudinaryOptions.SuppliersFolder,
                        ct);
                    photoId = uploadResult.Url;
                    _logger.LogInformation("Photo uploaded successfully for supplier: {Name}", dto.Name);
                }

                var supplier = new Supplier(
                    id: Guid.NewGuid(),
                    name: dto.Name,
                    address: dto.Address,
                    phone: dto.Phone,
                    email: dto.Email,
                    photoId: photoId,
                    createdBy: createdBy
                );

                await _uow.Suppliers.AddAsync(supplier);
                await _uow.SaveChangesAsync();

                _logger.LogInformation("Supplier created successfully: {SupplierId} ({Name})", supplier.Id, supplier.Name);
                return supplier;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating supplier: {Name} by {CreatedBy}", dto.Name, createdBy);
                throw;
            }
        }

        public async Task<Supplier?> PatchAsync(Guid id, UpdateSupplierDto dto, CancellationToken ct = default)
        {
            _logger.LogInformation("Updating supplier: {SupplierId}", id);
            try
            {
                var supplier = await _uow.Suppliers.GetByIdAsync(id);
                if (supplier is null)
                {
                    _logger.LogWarning("Supplier not found for update: {SupplierId}", id);
                    return null;
                }

                var name = dto.Name ?? supplier.Name;
                var address = dto.Address ?? supplier.Address;
                var phone = dto.Phone ?? supplier.Phone;
                var email = dto.Email ?? supplier.Email;

                supplier.UpdateInfo(name, address, phone, email);
                _uow.Suppliers.Update(supplier);
                await _uow.SaveChangesAsync();

                _logger.LogInformation("Supplier updated successfully: {SupplierId}", id);
                return supplier;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating supplier: {SupplierId}", id);
                throw;
            }
        }

        public async Task<Supplier?> PatchPhotoAsync(Guid id, string? photoId, CancellationToken ct = default)
        {
            _logger.LogInformation("Updating photo for supplier: {SupplierId}", id);
            try
            {
                var supplier = await _uow.Suppliers.GetByIdAsync(id);
                if (supplier is null)
                {
                    _logger.LogWarning("Supplier not found for photo update: {SupplierId}", id);
                    return null;
                }

                supplier.SetPhoto(photoId);
                _uow.Suppliers.Update(supplier);
                await _uow.SaveChangesAsync();

                _logger.LogInformation("Supplier photo updated successfully: {SupplierId}", id);
                return supplier;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating supplier photo: {SupplierId}", id);
                throw;
            }
        }

        public async Task<Supplier?> ApproveAsync(Guid id, string approvedBy, CancellationToken ct = default)
        {
            _logger.LogInformation("Approving supplier: {SupplierId} by {ApprovedBy}", id, approvedBy);
            try
            {
                var supplier = await _uow.Suppliers.GetByIdAsync(id, ct);
                if (supplier is null)
                {
                    _logger.LogWarning("Supplier not found for approval: {SupplierId}", id);
                    return null;
                }

                supplier.Approve(approvedBy);
                _uow.Suppliers.Update(supplier);
                await _uow.SaveChangesAsync();

                _logger.LogInformation("Supplier approved successfully: {SupplierId} by {ApprovedBy}", id, approvedBy);
                return supplier;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error approving supplier: {SupplierId}", id);
                throw;
            }
        }

        public async Task<bool> DeleteAsync(Guid id, string deletedBy, CancellationToken ct = default)
        {
            _logger.LogInformation("Soft deleting supplier: {SupplierId} by {DeletedBy}", id, deletedBy);
            try
            {
                var supplier = await _uow.Suppliers.GetByIdAsync(id);
                if (supplier is null)
                {
                    _logger.LogWarning("Supplier not found for deletion: {SupplierId}", id);
                    return false;
                }

                supplier.SoftDelete(deletedBy);
                _uow.Suppliers.Update(supplier);
                await _uow.SaveChangesAsync();

                _logger.LogInformation("Supplier soft deleted successfully: {SupplierId} by {DeletedBy}", id, deletedBy);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting supplier: {SupplierId}", id);
                throw;
            }
        }

        public async Task<bool> RestoreAsync(Guid id, CancellationToken ct = default)
        {
            _logger.LogInformation("Restoring deleted supplier: {SupplierId}", id);
            try
            {
                var supplier = await _uow.Suppliers.IgnoreQueryFilters()
                    .FirstOrDefaultAsync(s => s.Id == id, ct);
                if (supplier is null)
                {
                    _logger.LogWarning("Deleted supplier not found for restoration: {SupplierId}", id);
                    return false;
                }

                supplier.Restore();
                _uow.Suppliers.Update(supplier);
                await _uow.SaveChangesAsync();

                _logger.LogInformation("Supplier restored successfully: {SupplierId}", id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error restoring supplier: {SupplierId}", id);
                throw;
            }
        }

        public async Task<BulkImportResultDto> BulkImportAsync(List<SupplierImportDto> suppliers, string importedBy, CancellationToken ct = default)
        {
            _logger.LogInformation("Starting bulk import of {Count} suppliers by {ImportedBy}", suppliers.Count, importedBy);
            try
            {
                var result = new BulkImportResultDto { TotalRows = suppliers.Count };
                var allSuppliers = await _uow.Suppliers.GetAllAsync();

                for (int i = 0; i < suppliers.Count; i++)
                {
                    var supplierDto = suppliers[i];
                    var rowNumber = i + 2;

                    var validationErrors = supplierDto.GetValidationErrors();
                    if (validationErrors.Any())
                    {
                        _logger.LogWarning("Validation error on row {RowNumber}: {Errors}",
                            rowNumber, string.Join(", ", validationErrors));
                        result.FailedImports++;
                        result.Errors.Add(new SupplierServiceNet.CrossCutting.Dtos.Excel.RowErrorDto
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
                            if (!Guid.TryParse(supplierDto.Id, out var supplierId))
                            {
                                _logger.LogWarning("Invalid GUID on row {RowNumber}: {Id}", rowNumber, supplierDto.Id);
                                result.FailedImports++;
                                result.Errors.Add(new SupplierServiceNet.CrossCutting.Dtos.Excel.RowErrorDto
                                {
                                    RowNumber = rowNumber,
                                    Identifier = supplierDto.Id ?? "Unknown",
                                    Errors = new List<string> { "Invalid ID format (must be GUID)" }
                                });
                                continue;
                            }

                            var existing = allSuppliers.FirstOrDefault(s => s.Id == supplierId);
                            if (existing == null)
                            {
                                _logger.LogWarning("Supplier not found for update on row {RowNumber}: {Id}", rowNumber, supplierId);
                                result.FailedImports++;
                                result.Errors.Add(new SupplierServiceNet.CrossCutting.Dtos.Excel.RowErrorDto
                                {
                                    RowNumber = rowNumber,
                                    Identifier = supplierDto.Id,
                                    Errors = new List<string> { $"Supplier with ID {supplierId} not found" }
                                });
                                continue;
                            }

                            existing.UpdateInfo(supplierDto.Name, supplierDto.Address, supplierDto.Phone, supplierDto.Email);
                            _uow.Suppliers.Update(existing);
                            _logger.LogDebug("Updated supplier on row {RowNumber}: {Id}", rowNumber, supplierId);
                        }
                        else
                        {
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
                            _logger.LogDebug("Created supplier on row {RowNumber}: {Name}", rowNumber, supplierDto.Name);
                        }

                        result.SuccessfulImports++;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error processing row {RowNumber}: {Name}", rowNumber, supplierDto.Name);
                        result.FailedImports++;
                        result.Errors.Add(new SupplierServiceNet.CrossCutting.Dtos.Excel.RowErrorDto
                        {
                            RowNumber = rowNumber,
                            Identifier = supplierDto.Id ?? supplierDto.Name ?? "Unknown",
                            Errors = new List<string> { $"Error processing row: {ex.Message}" }
                        });
                    }
                }

                if (result.SuccessfulImports > 0)
                {
                    await _uow.SaveChangesAsync();
                    _logger.LogInformation("Bulk import completed: {Successful} successful, {Failed} failed out of {Total}",
                        result.SuccessfulImports, result.FailedImports, result.TotalRows);
                }

                result.Message = result.FailedImports == 0
                    ? $"Successfully imported {result.SuccessfulImports} suppliers"
                    : $"Imported {result.SuccessfulImports} of {result.TotalRows} suppliers. {result.FailedImports} failed.";

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during bulk import by {ImportedBy}", importedBy);
                throw;
            }
        }

        public async Task<List<SupplierExportDto>> GetForExportAsync(CancellationToken ct = default)
        {
            _logger.LogInformation("Exporting suppliers");
            try
            {
                var suppliers = await _uow.Suppliers.GetAllAsync();
                var result = suppliers.Select(s => new SupplierExportDto
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

                _logger.LogInformation("Successfully exported {Count} suppliers", result.Count);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting suppliers");
                throw;
            }
        }
    }
}
