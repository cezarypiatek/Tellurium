using System.Collections.Generic;
using Tellurium.VisualAssertions.Infrastructure;

namespace Tellurium.VisualAssertions.Screenshots.Domain
{
    public class ScreenshotData : Entity
    {
        public virtual string Hash { get; set; }
        public virtual byte[] Image { get; set; }

        public override int GetHashCode()
        {
            return Hash.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var screenshotDataObj = obj as ScreenshotData;
            if (screenshotDataObj == null)
            {
                return false;
            }
            
            return this.Hash == screenshotDataObj.Hash;
        }

        public virtual void UpdateHash(IReadOnlyList<BlindRegion> blindRegions)
        {
            this.Hash = ImageHelpers.ComputeHash(this.Image, blindRegions);
        }
    }
}