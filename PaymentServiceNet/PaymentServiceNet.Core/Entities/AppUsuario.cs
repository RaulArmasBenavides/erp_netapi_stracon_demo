using Microsoft.AspNetCore.Identity;

namespace PaymentServiceNet.Core.Entities
{
    public class AppUsuario : IdentityUser
    {

        public string Name { get; set; }
    }
}
