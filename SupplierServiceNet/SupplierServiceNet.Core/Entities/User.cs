using Microsoft.AspNetCore.Identity;

namespace SupplierServiceNet.Core.Entities
{
    public class User : IdentityUser
    {

        public DateTime CreatedAt { get; set; }
        // Navegación a roles
        public virtual ICollection<IdentityUserRole<string>> UserRoles { get; set; }
    }
}
