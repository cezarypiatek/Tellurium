using System.Collections.Generic;
using System.Linq;
using Tellurium.VisualAssertions.Infrastructure;

namespace Tellurium.VisualAssertion.Dashboard.Services.TestCase.Queries
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
                .OrderBy(x=>x.PatternScreenshotName)
                .ToList();
        }
    }
}