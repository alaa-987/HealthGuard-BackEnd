using HealthGuard.Core.Entities;
using HealthGuard.Core.Repository.contract;
using HealthGuard.Core.Specifications;
using HealthGurad.Repository.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthGurad.Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
    {
        private readonly StoreContext _dbContext;

        public GenericRepository(StoreContext dbContext)
        {
            _dbContext = dbContext;
        }
        private IQueryable<T> ApplySpecifactions(ISpecification<T> spec)
        {
            return SpecificationEvaluator<T>.GetQuery(_dbContext.Set<T>(), spec);
        }
        public async Task<IReadOnlyCollection<T>> GetAllAsync()
        {
            if (typeof(T) == typeof(Product))
            {
                return (IReadOnlyCollection<T>)await _dbContext.Set<Product>().Include(p => p.Category).ToListAsync();
            }
            return await _dbContext.Set<T>().ToListAsync();
        }

        public async Task<IReadOnlyList<T>> GetAllWithSpecAsync(ISpecification<T> spec)
        {
            return await ApplySpecifactions(spec).ToListAsync();
        }

        public async Task<T?> GetAsync(int id)
        {
            if (typeof(T) == typeof(Product))
            {
                return await _dbContext.Set<Product>().Where(p => p.Id == id).Include(p => p.Category).FirstOrDefaultAsync() as T;
            }
            return await _dbContext.Set<T>().FindAsync(id);
        }

        public async Task<int> GetCountAsync(ISpecification<T> spec)
        {
            return await ApplySpecifactions(spec).CountAsync();
        }

        public async Task<T?> GetWithSpecAsync(ISpecification<T> spec)
        {
            return await ApplySpecifactions(spec).FirstOrDefaultAsync();
        }
        public async Task Add(T item)
         => await _dbContext.Set<T>().AddAsync(item);

        public void Update(T item)
           => _dbContext.Set<T>().Update(item);

        public void Delete(T item)
          => _dbContext.Set<T>().Remove(item);
    }
}
