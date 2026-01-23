using System.ComponentModel.DataAnnotations;

namespace PaymentServiceNet.Core.Entities
{
    public class Category
    {
        [Key]
        public int  Id { get; set; }

        [Required]
        public string Nombre { get; set; }
        public DateTime FechaCreacion { get; set; } = DateTime.Now;
    }
}
