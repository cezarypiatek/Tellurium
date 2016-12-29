using System.IO;
using MaintainableSelenium.MvcPages;
using MaintainableSelenium.MvcPages.BrowserCamera;
using MaintainableSelenium.MvcPages.BrowserCamera.Lens;
using MaintainableSelenium.MvcPages.SeleniumUtils;
using MaintainableSelenium.Sample.Website.Mvc;
using MaintainableSelenium.VisualAssertions.Screenshots;
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
            //Initialize MvcPages
            var browserAdapterConfig = new BrowserAdapterConfig()
            {
                BrowserType = driverType,
                SeleniumDriversPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "Drivers"),
                PageUrl = "http://localhost:51767",
                ScreenshotsPath = @"c:\selenium\",
                BrowserDimensions = new BrowserDimensionsConfig
                {
                    Width = 1200,
                    Height = 768
                },
                BrowserCameraConfig = new BrowserCameraConfig
                {
                    LensType = LensType.Regular
                }
            };

            //Initialize VisualAssertions
            AssertView.Init(new VisualAssertionsConfig
            {
                BrowserName = driverType.ToString(),
                ProjectName = "Sample Project",
                ScreenshotCategory = "Sample Form",
            });


            //Prepare infrastructure for test
            using (var browserAdapter = BrowserAdapter.Create(browserAdapterConfig))
            {
                //Test
                browserAdapter.NavigateTo<TestFormsController>(c => c.Index());
                AssertView.EqualsToPattern(browserAdapter, "Sample1");
                
                browserAdapter.SaveScreenshot("Test");
                var detinationForm = browserAdapter.GetForm<SampleFormViewModel>(FormsIds.TestFormDst);
                var sourcenForm = browserAdapter.GetForm<SampleFormViewModel>(FormsIds.TestFormSrc);

                var textInputValue = sourcenForm.GetFieldValue(x=>x.TextInput);
                detinationForm.SetFieldValue(x => x.TextInput, textInputValue);

                var textAreaValue = sourcenForm.GetFieldValue(x => x.TextAreaInput);
                detinationForm.SetFieldValue(x => x.TextAreaInput, textAreaValue);

                var passwordValue = sourcenForm.GetFieldValue(x => x.PasswordInput);
                detinationForm.SetFieldValue(x => x.PasswordInput, passwordValue);


                var checkboxValue = sourcenForm.GetFieldValue(x=>x.CheckboxInput);
                detinationForm.SetFieldValue(x => x.CheckboxInput, checkboxValue);

                var selectListValue = sourcenForm.GetFieldValue(x=>x.SelectListValue);
                detinationForm.SetFieldValue(x=>x.SelectListValue, selectListValue);

                AssertView.EqualsToPattern(browserAdapter, "Sample2");
            }
        }

        [Test]
        public void should_be_able_to_run_test_with_configuration_from_file()
        {
            //Initialize MvcPages
            var browserAdapterConfig = BrowserAdapterConfig.FromAppConfig(TestContext.CurrentContext.TestDirectory);

            //Initialize VisualAssertions
            AssertView.Init(new VisualAssertionsConfig
            {
                BrowserName = browserAdapterConfig.BrowserType.ToString(),
                ProjectName = "Sample Project",
                ScreenshotCategory = "Sample Form",
            });


            //Prepare infrastructure for test
            using (var browserAdapter = BrowserAdapter.Create(browserAdapterConfig))
            {
                //Test
                browserAdapter.NavigateTo<TestFormsController>(c => c.Index());
                AssertView.EqualsToPattern(browserAdapter, "Sample21");
                
                browserAdapter.SaveScreenshot("Test");
                var detinationForm = browserAdapter.GetForm<SampleFormViewModel>(FormsIds.TestFormDst);
                var sourcenForm = browserAdapter.GetForm<SampleFormViewModel>(FormsIds.TestFormSrc);

                var textInputValue = sourcenForm.GetFieldValue(x=>x.TextInput);
                detinationForm.SetFieldValue(x => x.TextInput, textInputValue);

                var textAreaValue = sourcenForm.GetFieldValue(x => x.TextAreaInput);
                detinationForm.SetFieldValue(x => x.TextAreaInput, textAreaValue);

                var passwordValue = sourcenForm.GetFieldValue(x => x.PasswordInput);
                detinationForm.SetFieldValue(x => x.PasswordInput, passwordValue);


                var checkboxValue = sourcenForm.GetFieldValue(x=>x.CheckboxInput);
                detinationForm.SetFieldValue(x => x.CheckboxInput, checkboxValue);

                var selectListValue = sourcenForm.GetFieldValue(x=>x.SelectListValue);
                detinationForm.SetFieldValue(x=>x.SelectListValue, selectListValue);

                AssertView.EqualsToPattern(browserAdapter, "Sample22");
            }
        }
    }
}