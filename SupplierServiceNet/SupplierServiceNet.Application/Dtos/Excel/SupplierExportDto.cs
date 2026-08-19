namespace SupplierServiceNet.Application.Dtos.Excel
{
    public class SupplierExportDto
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
        public string? Address { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? Status { get; set; }
    }
}
