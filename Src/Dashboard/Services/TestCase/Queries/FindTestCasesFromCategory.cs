using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;
using Tellurium.VisualAssertions.Infrastructure;

namespace Tellurium.VisualAssertion.Dashboard.Services.TestCase.Queries
{
    public class FindTestCasesFromCategory:IQueryAll<VisualAssertions.Screenshots.Domain.TestCase>
    {
        private readonly long categoryId;

        public FindTestCasesFromCategory(long categoryId)
        {
            this.categoryId = categoryId;
        }

        public List<VisualAssertions.Screenshots.Domain.TestCase> GetQuery(IQueryable<VisualAssertions.Screenshots.Domain.TestCase> query)
        {
            return query.Where(x => x.Category.Id == categoryId)
                .FetchMany(x=>x.Patterns)
                .OrderBy(x=>x.PatternScreenshotName)
                .ToList();
        }
    }
}