using System.ComponentModel.DataAnnotations;

namespace PaymentServiceNet.Application.Dtos
{
    public class LoginUserDto
    {
        [Required(ErrorMessage = "El usuario es obligatorio")]
        public string NombreUsuario { get; set; }
        [Required(ErrorMessage = "El password es obligatorio")]      
        public string Password { get; set; }
       
    }
}
