using System.Collections.Generic;
using System.Linq;

namespace Tellurium.VisualAssertions.Infrastructure.Persistence
{
    public class Repository<TEntity>: IRepository<TEntity> where TEntity : Entity
    {
        private readonly ISessionContext sessionContext;

        public Repository(ISessionContext sessionContext)
        {
            this.sessionContext = sessionContext;
        }

        public TEntity Get(long id)
        {
            return sessionContext.Session.Get<TEntity>(id);
        }

        public List<TEntity> GetAll()
        {
            return sessionContext.Session.Query<TEntity>().ToList();
        }

        public void Save(TEntity entity)
        {
            sessionContext.Session.SaveOrUpdate(entity);
        }

        public TEntity FindOne(IQueryOne<TEntity> queryOne)
        {
            return queryOne.GetQuery(sessionContext.Session.Query<TEntity>());
        }

        public List<TEntity> FindAll(IQueryAll<TEntity> queryAll)
        {
            if (queryAll == null)
            {
                return sessionContext.Session.Query<TEntity>().ToList();
            }
            return queryAll.GetQuery(sessionContext.Session.Query<TEntity>());
        }
    }
}