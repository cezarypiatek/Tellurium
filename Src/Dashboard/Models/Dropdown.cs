using System.Collections.Generic;

namespace Tellurium.VisualAssertion.Dashboard.Models
{
    public class Dropdown<TValue>
    {
        public TValue Selected { get; set; }
        public List<DropdownItem<TValue>> Options { get; set; }
    }

    public class DropdownItem<TValue>
    {
        public string Label { get; set; }
        public TValue Value { get; set; }
    }
}