using System.Collections.Generic;
using System.Linq;
using MaintainableSelenium.Toolbox.Infrastructure;

namespace MaintainableSelenium.Web.Services.TestCase.Queries
{
    public class FindTestCasesFromCategory:IQueryAll<Toolbox.Screenshots.Domain.TestCase>
    {
        private readonly long categoryId;

        public FindTestCasesFromCategory(long categoryId)
        {
            this.categoryId = categoryId;
        }

        public List<Toolbox.Screenshots.Domain.TestCase> GetQuery(IQueryable<Toolbox.Screenshots.Domain.TestCase> query)
        {
            return query.Where(x => x.Category.Id == categoryId).ToList();
        }
    }
}