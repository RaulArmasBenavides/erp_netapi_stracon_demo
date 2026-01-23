using ApiMovies.Core.Entities;
using ApiMovies.Core.IRepositorio;
using ApiMovies.Infrastructure.Data;
using ApiMovies.Infrastructure.Repositorio;

namespace ApiMovies.Repositorio
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        private readonly ApplicationDbContext _bd;

        public CategoryRepository(ApplicationDbContext bd) : base(bd)
        {
            _bd = bd;
        }

        public bool ActualizarCategoria(Category categoria)
        {
            categoria.FechaCreacion = DateTime.Now;
            _bd.Categoria.Update(categoria);
            return Guardar();
        }

        public bool BorrarCategoria(Category categoria)
        {
            _bd.Categoria.Remove(categoria);
            return Guardar();
        }

        public bool CrearCategoria(Category categoria)
        {
            categoria.FechaCreacion = DateTime.Now;
            _bd.Categoria.Add(categoria);
            return Guardar();
        }

        public bool ExisteCategoria(string nombre)
        {
            bool valor = _bd.Categoria.Any(c => c.Nombre.ToLower().Trim() == nombre.ToLower().Trim());
            return valor;
        }

        public bool ExisteCategoria(int id)
        {
            return _bd.Categoria.Any(c => c.Id == id);
        }

        public Category GetCategoria(int categorId)
        {
            return _bd.Categoria.FirstOrDefault(c => c.Id == categorId);
        }

        public ICollection<Category> GetCategorias()
        {
            return _bd.Categoria.OrderBy(c => c.Nombre).ToList();
        }

        public bool Guardar()
        {
            return _bd.SaveChanges() >= 0 ;
        }
    }
}
