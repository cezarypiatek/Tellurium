using System.Linq;

namespace Tellurium.VisualAssertions.Infrastructure
{
    public interface IQueryOne<TEntity> where TEntity : Entity
    {
        TEntity GetQuery(IQueryable<TEntity> query);
    }
}