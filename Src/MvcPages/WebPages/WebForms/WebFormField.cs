using System;
using System.Collections.Generic;
using System.Linq;
using OpenQA.Selenium;
using Tellurium.MvcPages.SeleniumUtils;
using Tellurium.MvcPages.WebPages.WebForms.FieldLocators;

namespace Tellurium.MvcPages.WebPages.WebForms
{
   

    public class WebFormField
    {
        private readonly IWebElement form;

        private readonly IWebFormFieldLocator fieldLocator;

        public string FieldName => this.FieldElement.GetAttribute("name");

        public IStableWebElement FieldElement { get; private set; }
        public IFormInputAdapter FieldAdapter { get; private set; }

        private readonly List<IFormInputAdapter> supportedImputAdapters;
        

        public WebFormField(IWebElement form, IWebFormFieldLocator fieldLocator, List<IFormInputAdapter> supportedImputAdapters)
        {
            this.form = form;
            this.fieldLocator = fieldLocator;
            this.supportedImputAdapters = supportedImputAdapters;
            BuildFieldAccessFacility();
        }

        private IFormInputAdapter GetFieldAdapter(IWebElement fieldElement)
        {
            var adapter = supportedImputAdapters.FirstOrDefault(x => x.CanHandle(fieldElement));
            if (adapter == null)
            {
                throw new NotSupportedException("Not supported form element");
            }
            return adapter;
        }

        private IStableWebElement FindFieldElement()
        {
            return fieldLocator.FindFieldElement(form);
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