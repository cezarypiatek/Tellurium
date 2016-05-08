using System.Linq;

namespace MaintainableSelenium.VisualAssertions.Infrastructure
{
    public interface IQueryOne<TEntity> where TEntity : Entity
    {
        TEntity GetQuery(IQueryable<TEntity> query);
    }
}