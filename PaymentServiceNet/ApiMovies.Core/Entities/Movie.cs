using System.ComponentModel.DataAnnotations.Schema;

namespace ApiMovies.Core.Entities
{
    public class Movie

    {
        public int Id { get; set; }
        public string Nombre { get; set; }

        public string RutaImagen { get; set; }
        public string Descripcion { get; set; }
        public int Duracion { get; set; }

        public string Estado { get; set; }

        public enum TipoClasificacion { Siete, Trece, Dieciseis, Dieciocho }

        public TipoClasificacion Clasificacion { get; set; }
        public DateTime FechaCreacion { get; set; }

        [ForeignKey("categoriaId")]
        public int categoriaId { get; set; }
        public Category Categoria { get; set; }
    }
}
