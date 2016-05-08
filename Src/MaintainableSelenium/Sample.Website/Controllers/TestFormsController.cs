using System.Collections.Generic;
using System.Web.Mvc;
using Microsoft.Web.Mvc;
using Sample.Website.Models;

namespace Sample.Website.Controllers
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
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult SaveSampleForm(SampleFormViewModel input)
        {
            return this.RedirectToAction(x => x.Index());
        }
    }
}