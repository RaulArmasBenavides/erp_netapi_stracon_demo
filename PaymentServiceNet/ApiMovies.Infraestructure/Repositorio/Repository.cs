using ApiMovies.Core.IRepositorio;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ApiMovies.Infrastructure.Repositorio
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly DbContext _context;
        internal DbSet<T> _dbset;

        public Repository(DbContext context)
        {
            _context = context;
            this._dbset = context.Set<T>();
        }

        public void Add(T entity)
        {
            _dbset.Add(entity);
        }

        public void Update(T entity)
        {
            _dbset.Update(entity);
        }

        public T Get(int id)
        {
            return _dbset.Find(id);
        }

        public IEnumerable<T> GetAll(Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, string includeProperties = null)
        {
            IQueryable<T> query = _dbset;

            if (filter != null)
            {
                query = query.Where(filter);

            }

            if (includeProperties != null)
            {
                foreach (var inc in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(inc);
                }

            }

            if (orderBy != null)
            {
                return orderBy(query).ToList();
            }

            return query.ToList();
        }

        public IEnumerable<object> GetAllSelectLoading()
        {
            throw new NotImplementedException();
        }

        public T GetFirstOrDefault(Expression<Func<T, bool>> filter = null, string includeProperties = null)
        {
            throw new NotImplementedException();
        }

        public void Remove(int id)
        {
            throw new NotImplementedException();
        }

        public void Remove(T entity)
        {
            _dbset.Remove(entity);
        }
        public bool Exists(Expression<Func<T, bool>> filter)
        {
            return _dbset.Any(filter);
        }
    }
}
