using ApiMovies.Core.IRepositorio;

namespace ApiMovies.Core.IRepositorio
{
    public interface IUnitOfWork : IDisposable
    {
        ICategoryRepository Categories { get; }
        IMovieRepository Movies { get; }
        IUserRepository Users { get; }
        void Save();
        Task<int> SaveChangesAsync();
    }
}
