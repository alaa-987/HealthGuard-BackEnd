using HealthGuard.Core;
using HealthGuard.Core.Entities;
using HealthGuard.Core.Repository.contract;
using HealthGurad.Repository.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthGurad.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly StoreContext _dbContext;
        private Hashtable _reposatories;

        public UnitOfWork(StoreContext dbContext)
        {
            _dbContext = dbContext;
            _reposatories = new Hashtable();
        }
        public async Task<int> CompleteAsync()
          => await _dbContext.SaveChangesAsync();

        public async ValueTask DisposeAsync()
           => await _dbContext.DisposeAsync();

        public IGenericRepository<TEntity> Repository<TEntity>() where TEntity : BaseEntity
        {
            var Type = typeof(TEntity).Name;
            if (!_reposatories.ContainsKey(Type))
            {
                var Repo = new GenericRepository<TEntity>(_dbContext);
                _reposatories.Add(Type, Repo);
            }
            return _reposatories[Type] as IGenericRepository<TEntity>;

        }
    }
}
