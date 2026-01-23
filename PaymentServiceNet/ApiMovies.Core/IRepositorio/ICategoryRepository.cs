using ApiMovies.Core.Entities;

namespace ApiMovies.Core.IRepositorio
{
    public interface ICategoryRepository : IRepository<Category>
    {
        ICollection<Category> GetCategorias();
        Category GetCategoria(int categorId);
        bool ExisteCategoria(string nombre);
        bool ExisteCategoria(int id);
        bool CrearCategoria(Category categoria);
        bool ActualizarCategoria(Category categoria);
        bool BorrarCategoria(Category categoria);
        bool Guardar();
    }
}
