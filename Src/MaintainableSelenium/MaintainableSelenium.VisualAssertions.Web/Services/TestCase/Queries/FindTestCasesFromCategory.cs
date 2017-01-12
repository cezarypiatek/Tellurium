using System.Collections.Generic;
using System.Linq;
using MaintainableSelenium.VisualAssertions.Infrastructure;

namespace MaintainableSelenium.VisualAssertions.Web.Services.TestCase.Queries
{
    public class FindTestCasesFromCategory:IQueryAll<Screenshots.Domain.TestCase>
    {
        private readonly long categoryId;

        public FindTestCasesFromCategory(long categoryId)
        {
            this.categoryId = categoryId;
        }

        public List<Screenshots.Domain.TestCase> GetQuery(IQueryable<Screenshots.Domain.TestCase> query)
        {
            return query.Where(x => x.Category.Id == categoryId)
                .OrderBy(x=>x.PatternScreenshotName)
                .ToList();
        }
    }
}