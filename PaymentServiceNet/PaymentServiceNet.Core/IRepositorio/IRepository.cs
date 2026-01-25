using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace PaymentServiceNet.Core.IRepositorio
{
    public interface IRepository<T> where T : class
    {

        void Add(T entity);
        Task AddAsync(T entity, CancellationToken ct = default);

        void Update(T entity);

        T Get(int id);
        Task<T?> GetAsync(int id, CancellationToken ct = default);

        IEnumerable<T> GetAll(
            Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            string includeProperties = null);

        Task<IReadOnlyList<T>> GetAllAsync(
            Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            string includeProperties = null,
            bool asNoTracking = true,
            CancellationToken ct = default);

        T GetFirstOrDefault(Expression<Func<T, bool>> filter = null, string includeProperties = null);
        Task<T?> GetFirstOrDefaultAsync(Expression<Func<T, bool>> filter = null, string includeProperties = null, bool asNoTracking = true, CancellationToken ct = default);

        void Remove(int id);
        void Remove(T entity);

        bool Exists(Expression<Func<T, bool>> filter);
        Task<bool> ExistsAsync(Expression<Func<T, bool>> filter, CancellationToken ct = default);
    }
}
