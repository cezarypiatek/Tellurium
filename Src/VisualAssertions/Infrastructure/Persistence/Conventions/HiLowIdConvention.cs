using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Instances;

namespace Tellurium.VisualAssertions.Infrastructure.Persistence.Conventions
{
    public class HiLowIdConvention:IIdConvention
    {
        public void Apply(IIdentityInstance instance)
        {
            instance.Column("Id");
            instance.GeneratedBy.HiLo("1000");
        }
    }
}