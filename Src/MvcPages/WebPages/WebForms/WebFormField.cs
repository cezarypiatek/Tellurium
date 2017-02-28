using System;
using System.Collections.Generic;
using System.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using Tellurium.MvcPages.SeleniumUtils;

namespace Tellurium.MvcPages.WebPages.WebForms
{
    public class WebFormField
    {
        private readonly IWebElement form;
        public string FieldName { get; }
        public IStableWebElement FieldElement { get; private set; }
        public IFormInputAdapter FieldAdapter { get; private set; }

        private readonly List<IFormInputAdapter> supportedImputAdapters;
        private readonly RemoteWebDriver driver;
        private static readonly TimeSpan InputSearchTimeout = TimeSpan.FromSeconds(30);
        

        public WebFormField(IWebElement form, string fieldName, List<IFormInputAdapter> supportedImputAdapters, RemoteWebDriver driver)
        {
            this.form = form;
            this.FieldName = fieldName;
            this.supportedImputAdapters = supportedImputAdapters;
            this.driver = driver;
            BuildFieldAccessFacility();
        }

        private IFormInputAdapter GetFieldAdapter(IWebElement fieldElement)
        {
            var adpter = supportedImputAdapters.FirstOrDefault(x => x.CanHandle(fieldElement));
            if (adpter == null)
            {
                throw new NotSupportedException("Not supported form element");
            }
            return adpter;
        }

        private IStableWebElement FindFieldElement()
        {
            return driver.FindStableWebElement(form, By.Name(FieldName), (int) InputSearchTimeout.TotalSeconds);
        }

        public void SetValue(string value)
        {
            FieldAdapter.SetValue(FieldElement, value);
        }

        public string GetValue()
        {
            return FieldAdapter.GetValue(FieldElement);
        }

        private void BuildFieldAccessFacility()
        {
            FieldElement = FindFieldElement();
            FieldAdapter = GetFieldAdapter(FieldElement);
        }
    }
}