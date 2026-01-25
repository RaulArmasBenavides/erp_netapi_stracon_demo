using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using PaymentServiceNet.Core.Interfaces;
using PaymentServiceNet.CrossCutting.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentServiceNet.Application.Services
{
    public sealed class CloudinaryService : ICloudinaryService
    {
        private readonly CloudinaryDotNet.Cloudinary _cld;
        private readonly CloudinaryOptions _opt;

        public CloudinaryService(IOptions<CloudinaryOptions> options)
        {
            _opt = options.Value;

            var account = new Account(_opt.CloudName, _opt.ApiKey, _opt.ApiSecret);
            _cld = new CloudinaryDotNet.Cloudinary(account)
            {
                Api = { Secure = true }
            };
        }

        public async Task<dynamic> UploadImageAsync(IFormFile file, string? folder = null, CancellationToken ct = default)
        {
            if (file == null || file.Length == 0) throw new ArgumentException("Archivo vacío.", nameof(file));

            // Guardar temporal para igualar tu flujo de Multer
            var tempPath = await SaveTempAsync(file, ct);

            try
            {
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(tempPath),
                    Folder = folder ?? _opt.DefaultFolder,
                    UseFilename = true,
                    UniqueFilename = true,
                    Overwrite = false,
                };

                var result = await _cld.UploadAsync(uploadParams);

                return MapResult(result);
            }
            finally
            {
                TryDeleteTemp(tempPath);
            }
        }

        public async Task<dynamic> UploadAnyFileAsync(IFormFile file, string? folder = null, CancellationToken ct = default)
        {
            if (file == null || file.Length == 0) throw new ArgumentException("Archivo vacío.", nameof(file));

            var tempPath = await SaveTempAsync(file, ct);

            try
            {
                var uploadParams = new RawUploadParams
                {
                    File = new FileDescription(tempPath),
                    Folder = folder ?? _opt.DefaultFolder,
                    UseFilename = true,
                    UniqueFilename = true,
                    Overwrite = false,
                };

                // RawUpload cubre PDF/otros. Si quieres auto-detect, también puedes usar AutoUploadParams.
                var result = await _cld.UploadAsync(uploadParams);

                return MapResult(result);
            }
            finally
            {
                TryDeleteTemp(tempPath);
            }
        }

        public async Task<bool> ResourceExistsAsync(string publicId, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(publicId)) return false;

            try
            {
                // Similar a cld.api.resource(publicId)
                var getParams = new GetResourceParams(publicId);
                var res = await _cld.GetResourceAsync(getParams);

                // Si llega aquí sin excepción, existe
                return res != null && string.Equals(res.PublicId, publicId, StringComparison.OrdinalIgnoreCase);
            }
 
            catch (Exception ex)
            {
                // Igual que tu Nest: si hay error externo, asumimos que existe para no mostrar genérica
                Console.Error.WriteLine($"Cloudinary ResourceExists error para {publicId}: {ex.Message}");
                return true;
            }
        }

        public async Task DeleteResourceAsync(string publicId, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(publicId)) return;

            var protectedIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                _opt.ProtectedRentingPublicId ?? string.Empty,
                _opt.ProtectedEmploymentPublicId ?? string.Empty,
                _opt.ProtectedSocialHelpPublicId ?? string.Empty,
            };

            if (protectedIds.Contains(publicId)) return;

            try
            {
                var delParams = new DeletionParams(publicId) { Invalidate = true };
                await _cld.DestroyAsync(delParams);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"[Cloudinary] No se pudo borrar {publicId}: {ex.Message}");
            }
        }

        public string GetEmailLogoUrl(int width = 120)
        {
            if (string.IsNullOrWhiteSpace(_opt.LogoPublicId))
                return string.Empty;

            // Equivalente a cloudinary.url(publicId, { width, quality:"auto", fetch_format:"auto", secure:true })
            var url = _cld.Api.UrlImgUp
                .Secure(true)
                .Transform(new Transformation()
                    .Width(width)
                    .Quality("auto")
                    .FetchFormat("auto"));

            return url.BuildUrl(_opt.LogoPublicId);
        }

        // ----------------- helpers -----------------

        private static async Task<string> SaveTempAsync(IFormFile file, CancellationToken ct)
        {
            var ext = Path.GetExtension(file.FileName);
            var tempPath = Path.Combine(Path.GetTempPath(), $"upload_{Guid.NewGuid():N}{ext}");

            await using var fs = new FileStream(tempPath, FileMode.CreateNew, FileAccess.Write, FileShare.None);
            await file.CopyToAsync(fs, ct);

            return tempPath;
        }

        private static void TryDeleteTemp(string tempPath)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(tempPath) && File.Exists(tempPath))
                    File.Delete(tempPath);
            }
            catch { /* ignora */ }
        }

        private static CloudinaryUploadResult MapResult(UploadResult result)
        {
            if (result == null)
                throw new InvalidOperationException("Cloudinary no devolvió resultado.");

            if (result.StatusCode != System.Net.HttpStatusCode.OK && result.StatusCode != System.Net.HttpStatusCode.Created)
                throw new InvalidOperationException($"Cloudinary upload falló: {result.Error?.Message}");

            return new CloudinaryUploadResult
            {
                PublicId = result.PublicId,
                Url = result.Url?.ToString() ?? string.Empty,
                SecureUrl = result.SecureUrl?.ToString() ?? string.Empty,
                Format = result.Format,
                Bytes = result.Bytes,
            };
        }
    }

    public sealed class CloudinaryUploadResult
    {
        public string PublicId { get; init; } = default!;
        public string SecureUrl { get; init; } = default!;
        public string Url { get; init; } = default!;
        public string ResourceType { get; init; } = default!;
        public string Format { get; init; } = default!;
        public long Bytes { get; init; }
    }
}
