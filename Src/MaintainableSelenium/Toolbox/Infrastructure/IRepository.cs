using System.Collections.Generic;

namespace MaintainableSelenium.Toolbox.Screenshots
{
    public interface IRepository<TEntity> where TEntity:Entity
    {
        TEntity Get(long id);
        void Save(TEntity entity);
        TEntity FindOne(IQueryOne<TEntity> queryOne);
        List<TEntity> FindAll(IQueryAll<TEntity> queryAll=null);
    }
}