using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupplierServiceNet.Core.Interfaces
{
    public interface ICloudinaryService
    {
        Task<dynamic> UploadImageAsync(IFormFile file, string? folder = null, CancellationToken ct = default);
        Task<dynamic> UploadAnyFileAsync(IFormFile file, string? folder = null, CancellationToken ct = default);

        Task<bool> ResourceExistsAsync(string publicId, CancellationToken ct = default);
        Task DeleteResourceAsync(string publicId, CancellationToken ct = default);

        string GetEmailLogoUrl(int width = 120);
    }

}
