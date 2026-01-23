using PaymentServiceNet.Core.IRepositorio;
using PaymentServiceNet.Infrastructure.Data;
using PaymentServiceNet.Repositorio;

namespace PaymentServiceNet.Infrastructure.Repositorio.WorkContainer
{
    public class UnitOfWork : IUnitOfWork
    {

        private readonly ApplicationDbContext db;
        public UnitOfWork( ApplicationDbContext mdb) {
            db = mdb;
            Categories = new CategoryRepository(mdb);
            Movies = new MovieRepository(mdb);
            Users = new UserRepository(mdb, null);
        }
        public ICategoryRepository Categories { get; private set; }
        public IMovieRepository Movies { get; private set; }
        public IUserRepository Users { get; private set; }
        public void Dispose()
        {
            db.Dispose();
        }
        public void Save()
        {
            db.SaveChanges();
        }
        public Task<int> SaveChangesAsync()
        {
            return db.SaveChangesAsync();
        }
    }
}
