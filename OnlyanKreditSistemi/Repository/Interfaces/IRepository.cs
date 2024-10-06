using OnlyanKreditSistemi.Models.BaseModels;

namespace OnlyanKreditSistemi.Repository.Interfaces
{
    public interface IRepository<T> where T : BaseEntity
    {
        Task<bool> AddAsync(T entity);
        Task AddRangeAsync(IEnumerable<T> entities);
        bool Update(T entity);
        bool Delete(T entity);
        void DeleteRange(IEnumerable<T> entities);
        Task<IQueryable<T>> GetAllAsync(System.Linq.Expressions.Expression<Func<T, bool>> predicate);
        Task<T?> GetAsync(System.Linq.Expressions.Expression<Func<T, bool>> predicate);
        Task<int> SaveAsync();
        int Save();
    }
}
