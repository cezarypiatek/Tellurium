using System.Linq;

namespace MaintainableSelenium.Toolbox.Screenshots
{
    public interface IQueryOne<TEntity> where TEntity : Entity
    {
        TEntity GetQuery(IQueryable<TEntity> query);
    }
}