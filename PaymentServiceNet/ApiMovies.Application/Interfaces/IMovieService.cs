using ApiMovies.Application.Dtos;
using ApiMovies.Core.Entities;
using ApiMovies.CrossCutting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiMovies.Application.Interfaces
{
    public interface IMovieService
    {
        Task CreateMovieAsync(Movie pel);
        Task UpdateMovieAsync(Movie pel);
        Task<bool> DeleteMovieAsync(int id);
        IEnumerable<Movie> GetAllReque();
        Movie GetPelicula(int id);
        bool ExistePelicula(int id);

        ApiResponse<PagedResult<MovieDto>> GetAllReque(int page, int pageSize);
    }
}
