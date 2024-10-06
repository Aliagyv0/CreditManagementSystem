using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using OnlyanKreditSistemi.Context;
using OnlyanKreditSistemi.Models.BaseModels;
using OnlyanKreditSistemi.Repository.Interfaces;
using System.Linq.Expressions;

namespace OnlyanKreditSistemi.Repository
{
    public class Repository<T> : IRepository<T> where T : BaseEntity
    {
        private readonly KreditDbContext _context;
        private readonly DbSet<T> _dbSet;
        public Repository(KreditDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }
        public async Task<bool> AddAsync(T entity)
        {
            EntityEntry Entityentry = await _dbSet.AddAsync(entity);
            return Entityentry.State == EntityState.Added;
        }

        public async Task AddRangeAsync(IEnumerable<T> entities)
        {
            await _dbSet.AddRangeAsync(entities);
        }

        public bool Delete(T entity)
        {
            EntityEntry Entityentry = _dbSet.Remove(entity);
            return Entityentry.State == EntityState.Deleted;
        }

        public void DeleteRange(IEnumerable<T> entities)
        {
            _dbSet.RemoveRange(entities);
        }

        public async Task<IQueryable<T>> GetAllAsync(Expression<Func<T, bool>> predicate)
        {
            return _dbSet.Where(predicate);
        }

        public async Task<T?> GetAsync(Expression<Func<T, bool>> predicate)
        {

            return await _dbSet.FirstOrDefaultAsync(predicate);
        }

        public int Save()
        {
            return _context.SaveChanges();
        }

        public async Task<int> SaveAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public bool Update(T entity)
        {
            EntityEntry Entityentry = _dbSet.Update(entity);
            return Entityentry.State == EntityState.Modified;
        }
    }
}
