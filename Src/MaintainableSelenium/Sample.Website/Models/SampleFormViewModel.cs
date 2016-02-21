using System.ComponentModel.DataAnnotations;

namespace Sample.Website.Models
{
    public class SampleFormViewModel
    {
        [Display(Name = "Text input")]
        public string TextInput { get; set; }

        [Display(Name = "Multiline input")]
        public string TextAreaInput { get; set; }

        [Display(Name = "Password input")]
        public string PasswordInput { get; set; }

        [Display(Name = "Checkbox input")]
        public bool CheckboxInput { get; set; }
    }
}