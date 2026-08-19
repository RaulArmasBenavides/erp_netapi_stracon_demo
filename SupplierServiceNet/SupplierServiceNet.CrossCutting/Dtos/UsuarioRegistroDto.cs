using System.ComponentModel.DataAnnotations;

namespace SupplierServiceNet.Application.Dtos
{
    public class UsuarioRegistroDto
    {
        [Required(ErrorMessage = "El nombre es obligatorio")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "El email es obligatorio")]
        public string Email { get; set; }

        [Required(ErrorMessage = "El password es obligatorio")]
        public string Password { get; set; }

        [Required(ErrorMessage = "La confirmación de contraseña es obligatoria")]
        public string ConfirmPassword { get; set; }

        public string Role { get; set; }
    }
}
