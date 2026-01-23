using PaymentServiceNet.Core.Entities;
using PaymentServiceNet.Core.IRepositorio;
using PaymentServiceNet.CrossCutting;
using PaymentServiceNet.Infrastructure.Data;
using PaymentServiceNet.Infrastructure.Repositorio;
using Microsoft.EntityFrameworkCore;

namespace PaymentServiceNet.Repositorio
{
    public class MovieRepository : Repository<Movie>, IMovieRepository
    {
        private readonly ApplicationDbContext bd;

        public MovieRepository(ApplicationDbContext mbd) : base(mbd)
        {
            bd = mbd;
        }
        public ICollection<Movie> SearchMovie(string nombre)
        {
            IQueryable<Movie> query = bd.Pelicula;

            if (!string.IsNullOrEmpty(nombre))
            {
                query = query.Where(e => e.Nombre.Contains(nombre) || e.Descripcion.Contains(nombre));
            }
            return query.ToList();
        }

        public bool CrearPelicula(Movie pelicula)
        {
            pelicula.FechaCreacion = DateTime.Now;
            bd.Pelicula.Add(pelicula);
            return Save();
        }

        public bool ExistePelicula(string nombre)
        {
            bool valor = bd.Pelicula.Any(c => c.Nombre.ToLower().Trim() == nombre.ToLower().Trim());
            return valor;
        }

        public bool ExistePelicula(int id)
        {
            return bd.Pelicula.Any(c => c.Id == id);
        }

        public Movie GetMovie(int peliculaId)
        {
            return bd.Pelicula.FirstOrDefault(c => c.Id == peliculaId);
        }

        public ICollection<Movie> GetMovies()
        {
            return bd.Pelicula.OrderBy(c => c.Nombre).ToList();
        }


        public PagedResult<Movie> GetMovies(int skip, int take)
        {
            var query = bd.Pelicula.AsQueryable();

            var total = query.Count();

            var items = query
                .OrderBy(c => c.Nombre)
                .Skip(skip)
                .Take(take)
                .ToList();

            return new PagedResult<Movie>
            {
                Items = items,
                TotalRows = total
            };
        }

        public ICollection<Movie> GetPeliculasEnCategoria(int catId)
        {
            return bd.Pelicula.Include(ca => ca.Categoria).Where(ca => ca.categoriaId == catId).ToList();
        }

        public bool Save()
        {
            return bd.SaveChanges() >= 0;
        }
    }
}
