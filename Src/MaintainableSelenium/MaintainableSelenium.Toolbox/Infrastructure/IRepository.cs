using System.Collections.Generic;

namespace MaintainableSelenium.Toolbox.Infrastructure
{
    public interface IRepository<TEntity> where TEntity : Entity
    {
        TEntity Get(long id);
        List<TEntity> GetAll();
        void Save(TEntity entity);
        TEntity FindOne(IQueryOne<TEntity> queryOne);
        List<TEntity> FindAll(IQueryAll<TEntity> queryAll = null);
    }
}