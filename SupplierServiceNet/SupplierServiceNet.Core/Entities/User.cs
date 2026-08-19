using Microsoft.AspNetCore.Identity;

namespace SupplierServiceNet.Core.Entities
{
    public class User : IdentityUser
    {
        public DateTime CreatedAt { get; set; }

        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
        public string? DeletedBy { get; set; }

        // Navegación a roles
        //public virtual ICollection<IdentityUserRole<string>> UserRoles { get; set; }
    }
}
