using Microsoft.AspNetCore.Http;
using SupplierServiceNet.CrossCutting.Dtos.Excel;

namespace SupplierServiceNet.Application.Interfaces
{
    public interface IExcelService
    {
        byte[] GenerateTemplateExcel();
        byte[] ExportSuppliersToExcel(List<SupplierExportDto> suppliers);
        Task<List<SupplierImportDto>> ReadSuppliersFromExcelAsync(IFormFile file);
    }
}
