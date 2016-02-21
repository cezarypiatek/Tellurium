using System.Web.Mvc;
using Microsoft.Web.Mvc;
using Sample.Website.Models;

namespace Sample.Website.Controllers
{
    public class TestFormsController:Controller
    {
        public ActionResult Index()
        {
            var viewModel = new SampleFormViewModel();
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult SaveSampleForm(SampleFormViewModel input)
        {
            return this.RedirectToAction(x => x.Index());
        }
    }
}