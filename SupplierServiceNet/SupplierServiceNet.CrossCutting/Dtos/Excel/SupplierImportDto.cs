namespace SupplierServiceNet.CrossCutting.Dtos.Excel
{
    public class SupplierImportDto
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
        public string? Address { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }

        public List<string> GetValidationErrors()
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(Name))
                errors.Add("Name is required");

            if (string.IsNullOrWhiteSpace(Email))
                errors.Add("Email is required");

            if (!string.IsNullOrWhiteSpace(Email) && !IsValidEmail(Email))
                errors.Add("Email format is invalid");

            if (string.IsNullOrWhiteSpace(Phone))
                errors.Add("Phone is required");

            return errors;
        }

        private static bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        public bool IsUpdate => !string.IsNullOrWhiteSpace(Id) && Guid.TryParse(Id, out _);
    }
}
