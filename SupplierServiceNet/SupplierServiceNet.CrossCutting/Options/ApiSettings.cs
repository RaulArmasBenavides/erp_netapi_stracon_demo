namespace SupplierServiceNet.CrossCutting.Options
{
    public class ApiSettings
    {
        public const string SectionName = "ApiSettings";
        public string Secreta { get; set; } = string.Empty;
        public int TokenExpirationDays { get; set; } = 7;
    }
}
