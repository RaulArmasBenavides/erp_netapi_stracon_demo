using System.ComponentModel.DataAnnotations;

namespace ApiPeliculas.Modelos.Dtos
{
    public class UsuarioLoginDto
    {
        [Required(ErrorMessage = "El usuario es obligatorio")]
        public string NombreUsuario { get; set; }
        [Required(ErrorMessage = "El nombre es obligatorio")]      
        public string Password { get; set; }
       
    }
}
