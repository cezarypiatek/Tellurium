using System.Drawing;
using Raven.Imports.Newtonsoft.Json;

namespace MaintainableSelenium.API.Models
{
    public class Test
    {
        public string Name { get; set; }
        public string ExpectedImageId { get; set; }
        public string ResultImageId { get; set; }

        [JsonIgnore]
        public Image ExpectedImage { get; set; }
        [JsonIgnore]
        public Image ResultImage { get; set; }
    }
}