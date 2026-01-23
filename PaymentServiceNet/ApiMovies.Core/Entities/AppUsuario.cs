using Microsoft.AspNetCore.Identity;

namespace ApiMovies.Core.Entities
{
    public class AppUsuario : IdentityUser
    {

        public string Name { get; set; }
    }
}
