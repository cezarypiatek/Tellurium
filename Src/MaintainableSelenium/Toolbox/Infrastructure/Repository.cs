using System.Collections.Generic;

namespace MaintainableSelenium.Toolbox.Screenshots
{
    public class Repository<TEntity>: IRepository<TEntity> where TEntity : Entity
    {
        public TEntity Get(long id)
        {
            throw new System.NotImplementedException();
        }

        public void Save(TEntity entity)
        {
            throw new System.NotImplementedException();
        }

        public TEntity FindOne(IQueryOne<TEntity> queryOne)
        {
            throw new System.NotImplementedException();
        }

        public List<TEntity> FindAll(IQueryAll<TEntity> queryAll)
        {
            throw new System.NotImplementedException();
        }
    }
}