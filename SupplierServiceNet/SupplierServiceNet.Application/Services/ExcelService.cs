using ClosedXML.Excel;
using Microsoft.AspNetCore.Http;
using SupplierServiceNet.CrossCutting.Dtos.Excel;
using SupplierServiceNet.Application.Interfaces;
using SupplierServiceNet.CrossCutting.Exceptions;

namespace SupplierServiceNet.Application.Services
{
    public class ExcelService : IExcelService
    {
        private const string SheetName = "Suppliers";
        private const string TemplateName = "Supplier_Template";

        public byte[] GenerateTemplateExcel()
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add(TemplateName);

            // Header row
            var headerRow = worksheet.Row(1);
            headerRow.Style.Font.Bold = true;
            headerRow.Style.Fill.BackgroundColor = XLColor.LightGray;

            worksheet.Cell(1, 1).Value = "ID (Leave empty for new)";
            worksheet.Cell(1, 2).Value = "Name *";
            worksheet.Cell(1, 3).Value = "Email *";
            worksheet.Cell(1, 4).Value = "Phone *";
            worksheet.Cell(1, 5).Value = "Address";

            // Set column widths
            worksheet.Column(1).Width = 30;
            worksheet.Column(2).Width = 25;
            worksheet.Column(3).Width = 30;
            worksheet.Column(4).Width = 15;
            worksheet.Column(5).Width = 35;

            // Add example row (grayed out for reference)
            worksheet.Row(2).Style.Font.Italic = true;
            worksheet.Cell(2, 1).Value = "(Optional for update)";
            worksheet.Cell(2, 2).Value = "Example Company";
            worksheet.Cell(2, 3).Value = "contact@example.com";
            worksheet.Cell(2, 4).Value = "+1234567890";
            worksheet.Cell(2, 5).Value = "123 Main Street, City";

            // Instructions
            worksheet.Row(4).Style.Font.Italic = true;
            worksheet.Cell(4, 1).Value = "Instructions:";
            worksheet.Cell(5, 1).Value = "- Fields marked with * are required";
            worksheet.Cell(6, 1).Value = "- Leave ID empty to create new supplier";
            worksheet.Cell(7, 1).Value = "- Provide ID (GUID) to update existing supplier";
            worksheet.Cell(8, 1).Value = "- Email must be valid format";

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }

        public byte[] ExportSuppliersToExcel(List<SupplierExportDto> suppliers)
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add(SheetName);

            // Header row
            var headerRow = worksheet.Row(1);
            headerRow.Style.Font.Bold = true;
            headerRow.Style.Fill.BackgroundColor = XLColor.FromHtml("#4472C4");
            headerRow.Style.Font.FontColor = XLColor.White;

            worksheet.Cell(1, 1).Value = "ID";
            worksheet.Cell(1, 2).Value = "Name";
            worksheet.Cell(1, 3).Value = "Email";
            worksheet.Cell(1, 4).Value = "Phone";
            worksheet.Cell(1, 5).Value = "Address";
            worksheet.Cell(1, 6).Value = "Status";
            worksheet.Cell(1, 7).Value = "Created By";
            worksheet.Cell(1, 8).Value = "Created At";

            // Set column widths
            worksheet.Column(1).Width = 30;
            worksheet.Column(2).Width = 25;
            worksheet.Column(3).Width = 30;
            worksheet.Column(4).Width = 15;
            worksheet.Column(5).Width = 35;
            worksheet.Column(6).Width = 15;
            worksheet.Column(7).Width = 20;
            worksheet.Column(8).Width = 20;

            // Data rows
            for (int i = 0; i < suppliers.Count; i++)
            {
                var supplier = suppliers[i];
                var rowNumber = i + 2;

                worksheet.Cell(rowNumber, 1).Value = supplier.Id;
                worksheet.Cell(rowNumber, 2).Value = supplier.Name;
                worksheet.Cell(rowNumber, 3).Value = supplier.Email;
                worksheet.Cell(rowNumber, 4).Value = supplier.Phone;
                worksheet.Cell(rowNumber, 5).Value = supplier.Address;
                worksheet.Cell(rowNumber, 6).Value = supplier.Status;
                worksheet.Cell(rowNumber, 7).Value = supplier.CreatedBy;
                worksheet.Cell(rowNumber, 8).Value = supplier.CreatedAt;

                // Format date column
                if (supplier.CreatedAt.HasValue)
                {
                    worksheet.Cell(rowNumber, 8).Style.DateFormat.Format = "yyyy-MM-dd HH:mm:ss";
                }
            }

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }

        public async Task<List<SupplierImportDto>> ReadSuppliersFromExcelAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ValidationException("File is empty or null");

            if (!file.FileName.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase))
                throw new ValidationException("File must be in .xlsx format");

            var suppliers = new List<SupplierImportDto>();

            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                stream.Position = 0;

                using var workbook = new XLWorkbook(stream);
                var worksheet = workbook.Worksheets.FirstOrDefault()
                    ?? throw new ValidationException("Workbook has no worksheets");

                var usedRange = worksheet.RangeUsed();
                if (usedRange == null)
                    throw new ValidationException("Excel file has no data rows (only headers)");

                var rows = usedRange.RowsUsed();
                var rowList = rows.ToList();

                if (rowList.Count <= 1)
                    throw new ValidationException("Excel file has no data rows (only headers)");

                for (int i = 1; i < rowList.Count; i++)
                {
                    var row = rowList[i];

                    // Skip empty rows
                    if (row.IsEmpty())
                        continue;

                    var supplier = new SupplierImportDto
                    {
                        Id = row.Cell(1).IsEmpty() ? null : row.Cell(1).Value.ToString()?.Trim(),
                        Name = row.Cell(2).IsEmpty() ? null : row.Cell(2).Value.ToString()?.Trim(),
                        Email = row.Cell(3).IsEmpty() ? null : row.Cell(3).Value.ToString()?.Trim(),
                        Phone = row.Cell(4).IsEmpty() ? null : row.Cell(4).Value.ToString()?.Trim(),
                        Address = row.Cell(5).IsEmpty() ? null : row.Cell(5).Value.ToString()?.Trim(),
                    };

                    suppliers.Add(supplier);
                }
            }

            return suppliers;
        }
    }
}
