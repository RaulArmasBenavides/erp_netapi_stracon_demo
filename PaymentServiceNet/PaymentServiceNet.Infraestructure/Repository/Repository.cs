using PaymentServiceNet.Core.IRepositorio;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace PaymentServiceNet.Infrastructure.Repositorio
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly DbContext _context;
        internal DbSet<T> _dbset;

        public Repository(DbContext context)
        {
            _context = context;
            _dbset = context.Set<T>();
        }

        public void Add(T entity)
        {
            _dbset.Add(entity);
        }

        public async Task AddAsync(T entity, CancellationToken ct = default)
        {
            await _dbset.AddAsync(entity, ct);
        }

        public void Update(T entity)
        {
            _dbset.Update(entity);
        }

        public T Get(int id)
        {
            return _dbset.Find(id);
        }

        public async Task<T?> GetAsync(int id, CancellationToken ct = default)
        {
            // FindAsync soporta CancellationToken con overload de object[]
            return await _dbset.FindAsync(new object[] { id }, ct);
        }

        public IEnumerable<T> GetAll(
            Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            string includeProperties = null)
        {
            IQueryable<T> query = _dbset;

            if (filter != null)
                query = query.Where(filter);

            if (!string.IsNullOrWhiteSpace(includeProperties))
            {
                foreach (var inc in includeProperties.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                    query = query.Include(inc.Trim());
            }

            if (orderBy != null)
                return orderBy(query).ToList();

            return query.ToList();
        }

        // NUEVO: versión async
        public async Task<IReadOnlyList<T>> GetAllAsync(
            Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            string includeProperties = null,
            bool asNoTracking = true,
            CancellationToken ct = default)
        {
            IQueryable<T> query = _dbset;

            if (asNoTracking)
                query = query.AsNoTracking();

            if (filter != null)
                query = query.Where(filter);

            if (!string.IsNullOrWhiteSpace(includeProperties))
            {
                foreach (var inc in includeProperties.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                    query = query.Include(inc.Trim());
            }

            if (orderBy != null)
                query = orderBy(query);

            return await query.ToListAsync(ct);
        }

        // IMPLEMENTADO: GetFirstOrDefault
        public T GetFirstOrDefault(
            Expression<Func<T, bool>> filter = null,
            string includeProperties = null)
        {
            IQueryable<T> query = _dbset;

            if (filter != null)
                query = query.Where(filter);

            if (!string.IsNullOrWhiteSpace(includeProperties))
            {
                foreach (var inc in includeProperties.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                    query = query.Include(inc.Trim());
            }

            return query.FirstOrDefault();
        }

        // Opcional: versión async del FirstOrDefault
        public async Task<T?> GetFirstOrDefaultAsync(
            Expression<Func<T, bool>> filter = null,
            string includeProperties = null,
            bool asNoTracking = true,
            CancellationToken ct = default)
        {
            IQueryable<T> query = _dbset;

            if (asNoTracking)
                query = query.AsNoTracking();

            if (filter != null)
                query = query.Where(filter);

            if (!string.IsNullOrWhiteSpace(includeProperties))
            {
                foreach (var inc in includeProperties.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                    query = query.Include(inc.Trim());
            }

            return await query.FirstOrDefaultAsync(ct);
        }

        // IMPLEMENTADO: Remove(int id)
        public void Remove(int id)
        {
            var entity = _dbset.Find(id);
            if (entity != null)
                _dbset.Remove(entity);
        }

        public void Remove(T entity)
        {
            _dbset.Remove(entity);
        }

        public bool Exists(Expression<Func<T, bool>> filter)
        {
            return _dbset.Any(filter);
        }

        public Task<bool> ExistsAsync(Expression<Func<T, bool>> filter, CancellationToken ct = default)
        {
            return _dbset.AnyAsync(filter, ct);
        }
    }
}
