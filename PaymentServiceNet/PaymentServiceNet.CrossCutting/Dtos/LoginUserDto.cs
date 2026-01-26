using System.ComponentModel.DataAnnotations;

namespace SupplierServiceNet.Application.Dtos
{
    public class LoginUserDto
    {
        [Required(ErrorMessage = "UserName is required")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "El password es obligatorio")]      
        public string Password { get; set; }
       
    }
}
