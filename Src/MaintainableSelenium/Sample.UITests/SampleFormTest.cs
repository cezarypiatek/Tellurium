using System.Collections.Generic;
using System.IO;
using MaintainableSelenium.Sample.Website.Mvc;
using MaintainableSelenium.Toolbox;
using MaintainableSelenium.Toolbox.Drivers;
using MaintainableSelenium.Toolbox.Screenshots;
using MaintainableSelenium.Toolbox.WebPages;
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
        [TestCase(SeleniumDriverType.Firefox)]
        [TestCase(SeleniumDriverType.Chrome)]
        [TestCase(SeleniumDriverType.InternetExplorer)]
        public void should_be_able_to_fill_sample_form(SeleniumDriverType driverType)
        {
            //Prepare infrastructure for test
            var driver = SeleniumDriverFactory.CreateLocalDriver(driverType, Path.Combine(TestContext.CurrentContext.TestDirectory, "Drivers"));
            driver.Manage().Window.Maximize();
            var camera = new BrowserCamera(driver, "SampleForm", "C:\\MaintainableSelenium\\screenshots", new List<BlindRegion>());
            var navigator = new Navigator(driver, "http://localhost:51767");

            //Test
            navigator.NavigateTo<TestFormsController>(c=>c.Index());
            camera.TakeScreenshot("Sample1");
            var form = driver.GetForm<SampleFormViewModel>(FormsIds.TestForm);
            form.SetFieldValue(x=>x.TextInput, "Hello world!!!");
            form.SetFieldValue(x=>x.TextAreaInput, "Long test message");
            form.SetFieldValue(x=>x.PasswordInput, "Secret_Password");
            form.SetFieldValue(x=>x.CheckboxInput, CheckboxFormInputAdapter.Checked);
            camera.TakeScreenshot("Sample2");

            //Clean up
            driver.Close();
            driver.Quit();
        }
    }
}