using Microsoft.Extensions.Options;
using SupplierServiceNet.Core.Entities;
using SupplierServiceNet.Core.Interfaces;
using SupplierServiceNet.Core.IRepositorio;
using SupplierServiceNet.CrossCutting.Options;
using SupplierServiceNet.CrossCutting.Supplier;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public async Task<Supplier> CreateAsync(CreateSupplierDto dto, CancellationToken ct = default)
        {
            string? photoId = null;

            // Si hay una imagen, subirla a Cloudinary
            if (dto.Photo != null)
            {
                // Usar el servicio de Cloudinary
                var uploadResult = await _cloudinaryService.UploadImageAsync(dto.Photo, _cloudinaryOptions.SuppliersFolder, ct);
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
                photoId: photoId  // Asignamos el public_id de Cloudinary
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
        public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
        {
            var supplier = await _uow.Suppliers.GetByIdAsync(id);
            if (supplier is null) return false;

            _uow.Suppliers.Remove(supplier);
            await _uow.SaveChangesAsync();

            return true;
        }
    }
}
