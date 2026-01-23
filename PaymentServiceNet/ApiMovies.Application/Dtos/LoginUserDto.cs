using System.ComponentModel.DataAnnotations;

namespace ApiMovies.Application.Dtos
{
    public class LoginUserDto
    {
        [Required(ErrorMessage = "El usuario es obligatorio")]
        public string NombreUsuario { get; set; }
        [Required(ErrorMessage = "El nombre es obligatorio")]      
        public string Password { get; set; }
       
    }
}
