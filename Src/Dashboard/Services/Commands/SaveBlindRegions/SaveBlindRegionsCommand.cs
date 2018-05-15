using Tellurium.VisualAssertion.Dashboard.Services.Commands.SaveBlindRegions;
using Tellurium.VisualAssertion.Dashboard.Services.WorkSeed;

namespace Tellurium.VisualAssertion.Dashboard.Services.Commands.SaveBlindRegion
{
    public class SaveBlindRegionsCommand:ICommand
    {
        public SaveLocalBlindRegionsDTO Local { get; set; }
        public SaveCategoryBlindRegionsDTO Category { get; set; }
        public SaveGlobalBlindRegionsDTO Global { get; set; }
    }
}