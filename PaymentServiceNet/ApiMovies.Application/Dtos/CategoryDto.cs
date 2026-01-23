using System.ComponentModel.DataAnnotations;

namespace ApiMovies.Application.Dtos
{
    public class CategoryDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [MaxLength(60, ErrorMessage = "El número máximo de caracteres es de 100!")]
        public string Nombre { get; set; }        
    }
}
