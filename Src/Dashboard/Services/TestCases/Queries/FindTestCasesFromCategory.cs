using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;
using Tellurium.VisualAssertions.Infrastructure;

namespace Tellurium.VisualAssertion.Dashboard.Services.TestCases.Queries
{
    public class FindTestCasesFromCategory:IQueryAll<Tellurium.VisualAssertions.Screenshots.Domain.TestCase>
    {
        private readonly long categoryId;

        public FindTestCasesFromCategory(long categoryId)
        {
            this.categoryId = categoryId;
        }

        public List<Tellurium.VisualAssertions.Screenshots.Domain.TestCase> GetQuery(IQueryable<Tellurium.VisualAssertions.Screenshots.Domain.TestCase> query)
        {
            return query.Where(x => x.Category.Id == categoryId)
                .FetchMany(x=>x.Patterns)
                .OrderBy(x=>x.PatternScreenshotName)
                .ToList();
        }
    }
}