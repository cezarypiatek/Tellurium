using System.Collections.Generic;
using System.Linq;
using NHibernate;
using NHibernate.Linq;

namespace MaintainableSelenium.Toolbox.Screenshots
{
    public class Repository<TEntity>: IRepository<TEntity> where TEntity : Entity
    {
        private readonly ISession session = PersistanceEngine.GetSession();

        public TEntity Get(long id)
        {
            return session.Get<TEntity>(id);
        }

        public List<TEntity> GetAll()
        {
            return session.Query<TEntity>().ToList();
        }

        public void Save(TEntity entity)
        {
            session.SaveOrUpdate(entity);
        }

        public TEntity FindOne(IQueryOne<TEntity> queryOne)
        {
            return queryOne.GetQuery(this.session.Query<TEntity>());
        }

        public List<TEntity> FindAll(IQueryAll<TEntity> queryAll)
        {
            if (queryAll == null)
            {
                return this.session.Query<TEntity>().ToList();
            }
            return queryAll.GetQuery(this.session.Query<TEntity>());
        }
    }
}