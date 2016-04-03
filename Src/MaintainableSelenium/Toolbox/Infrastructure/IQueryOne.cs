using System.Linq;

namespace MaintainableSelenium.Toolbox.Infrastructure
{
    public interface IQueryOne<TEntity> where TEntity : Entity
    {
        TEntity GetQuery(IQueryable<TEntity> query);
    }
}