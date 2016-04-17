using System.IO;
using MaintainableSelenium.Sample.Website.Mvc;
using MaintainableSelenium.Toolbox;
using MaintainableSelenium.Toolbox.SeleniumUtils;
using MaintainableSelenium.Toolbox.WebPages.WebForms.DefaultInputAdapters;
using NUnit.Framework;
using Sample.Website.Controllers;
using Sample.Website.Models;

namespace MaintainableSelenium.Sample.UITests
{
    //INFO: Run Sample.Website and detach debugger before running test
    [TestFixture, Explicit]
    public class SampleFormTest
    {
        [TestCase(BrowserType.Firefox)]
        [TestCase(BrowserType.Chrome)]
        [TestCase(BrowserType.InternetExplorer)]
        public void should_be_able_to_fill_sample_form(BrowserType driverType)
        {
            //Prepare infrastructure for test
            var browserAdapterConfig = new BrowserAdapterConfig()
            {
                BrowserType = driverType,
                PageUrl = "http://localhost:51767",
                ProjectName = "Sample Project",
                ScreenshotCategory = "Sample Form",
                SeleniumDriversPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "Drivers"),
                BrowserDimensions = new BrowserDimensionsConfig
                {
                    Width = 1200,
                    Height = 768
                }
            };

            using (var browserAdapter = BrowserAdapter.Create(browserAdapterConfig))
            {

                //Test
                browserAdapter.NavigateTo<TestFormsController>(c => c.Index());
                browserAdapter.TakeScreenshot("Sample1");
                
                var form = browserAdapter.GetForm<SampleFormViewModel>(FormsIds.TestForm);
                form.SetFieldValue(x => x.TextInput, "Hello world!!!");
                form.SetFieldValue(x => x.TextAreaInput, "Long test message");
                form.SetFieldValue(x => x.PasswordInput, "Secret_Password");
                form.SetFieldValue(x => x.CheckboxInput, CheckboxFormInputAdapter.Checked);
                
                browserAdapter.TakeScreenshot("Sample2");
            }
        }
    }
}