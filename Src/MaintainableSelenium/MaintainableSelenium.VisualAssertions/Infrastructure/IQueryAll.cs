using System.Collections.Generic;
using System.Linq;

namespace MaintainableSelenium.VisualAssertions.Infrastructure
{
    public interface IQueryAll<TEntity> where TEntity:Entity
    {
        List<TEntity> GetQuery(IQueryable<TEntity> query);
    }
}