namespace SupplierServiceNet.CrossCutting.Dtos.Excel
{
    public class BulkImportResultDto
    {
        public int TotalRows { get; set; }
        public int SuccessfulImports { get; set; }
        public int FailedImports { get; set; }
        public List<RowErrorDto> Errors { get; set; } = new();
        public string? Message { get; set; }
    }

    public class RowErrorDto
    {
        public int RowNumber { get; set; }
        public string? Identifier { get; set; }
        public List<string> Errors { get; set; } = new();
        public string ErrorSummary => string.Join("; ", Errors);
    }
}
