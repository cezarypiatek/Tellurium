using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Tellurium.Sample.Website.Models
{
    public class SampleFormViewModel
    {
        public readonly IReadOnlyList<SelectListItem> SelectListOptions = new List<SelectListItem>()
        {
             new SelectListItem()
            {
                Text = "----Select option---",
                Value = "x"
            },

            new SelectListItem()
            {
                Text = "Option1",
                Value = "Option1Value"
            },
            new SelectListItem()
            {
                Text = "Option2",
                Value = "Option2Value"
            },
            new SelectListItem()
            {
                Text = "Option3",
                Value = "Option3Value"
            }
        };
        
        public readonly IReadOnlyList<SelectListItem> MultiSelectListOptions = new List<SelectListItem>()
        {
             new SelectListItem()
            {
                Text = "----Select option---",
                Value = "x"
            },

            new SelectListItem()
            {
                Text = "Option1",
                Value = "Option1Value"

            },
            new SelectListItem()
            {
                Text = "Option2",
                Value = "Option2Value"
            },
            new SelectListItem()
            {
                Text = "Option3",
                Value = "Option3Value"
            }
        };

        [Display(Name = "Text input")]
        public string TextInput { get; set; }

        [Display(Name = "Multiline input")]
        public string TextAreaInput { get; set; }

        [Display(Name = "Password input")]
        public string PasswordInput { get; set; }

        [Display(Name = "Checkbox input")]
        public bool CheckboxInput { get; set; }

        [Display(Name = "Select input")]
        public string SelectListValue { get; set; }     
        
        [Display(Name = "MultiSelect input")]
        public string MultiSelectListValue { get; set; }
    }


    public class FormRetypeViewModel
    {
        public SampleFormViewModel SourceForm { get; set; }
        public SampleFormViewModel DestinationForm { get; set; }
    }
}