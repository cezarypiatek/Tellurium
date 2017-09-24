using System.Web.Mvc;
using Microsoft.Web.Mvc;
using Tellurium.Sample.Website.Models;

namespace Tellurium.Sample.Website.Controllers
{
    public class TestFormsController:Controller
    {
        public ActionResult Index()
        {
            var viewModel = new FormRetypeViewModel()
            {
                SourceForm = new SampleFormViewModel()
                {
                    TextInput = "Hello text input",
                    TextAreaInput = "Hello textarea input",
                    PasswordInput = "Hello secret password",
                    CheckboxInput = true,
                    SelectListValue =  "Option2Value"
                },
                DestinationForm = new SampleFormViewModel()
                {
                    SelectListValue = "x"
                }
            };
            viewModel.SourceForm.MultiSelectListOptions[1].Selected = true;
            viewModel.SourceForm.MultiSelectListOptions[2].Selected = true;
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult SaveSampleForm(SampleFormViewModel input)
        {
            return this.RedirectToAction(x => x.Index());
        }
    }
}