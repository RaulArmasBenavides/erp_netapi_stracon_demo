using ApiMovies.Core.Entities;
using ApiMovies.CrossCutting;

namespace ApiMovies.Core.IRepositorio
{
    public interface IMovieRepository : IRepository<Movie>
    {
        ICollection<Movie> GetMovies();
        Movie GetMovie(int peliculaId);
        ICollection<Movie> GetPeliculasEnCategoria(int catId);
        ICollection<Movie> SearchMovie(string nombre);
        bool Save();

        PagedResult<Movie> GetMovies(int skip, int take);
    }
}
