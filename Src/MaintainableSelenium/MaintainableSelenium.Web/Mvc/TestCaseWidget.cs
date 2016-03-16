using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Web.Mvc;
using MaintainableSelenium.Toolbox.Screenshots;
using MaintainableSelenium.Web.Controllers;

namespace MaintainableSelenium.Web.Mvc
{
    public class TestCaseWidget : JqueryUIWidget
    {
        private readonly UrlHelper urlHelper;

        public TestCaseWidget(UrlHelper urlHelper) : base("testcase")
        {
            this.urlHelper = urlHelper;
        }

        public void SetBrowserName(string browserName)
        {
            this.SetOption("browser", browserName);
        }

        public void SetTestCaseId(string testCaseId)
        {
            this.SetOption("id", testCaseId);
        }

        public void SetSaveLocalBlindspotsAction<TController>(Expression<Action<TController>> action)
            where TController : Controller
        {
            this.SetOption("actionSaveLocal", urlHelper.ActionFor(action));
        }

        public void SetSaveGlobalBlindspotsAction<TController>(Expression<Action<TController>> action)
            where TController : Controller
        {
            this.SetOption("actionSaveGlobal", urlHelper.ActionFor(action));
        }

        public void SetLocalBlindspots(List<BlindRegion> blindRegions)
        {
            this.SetComplexOption("localRegions", blindRegions);
        }

        public void SetGlobalBlindspots(List<BlindRegion> blindRegions)
        {
            this.SetComplexOption("globalRegions", blindRegions);
        }

        public static TestCaseWidget Create<TController>(Expression<Action<TController>> actionSaveLocal,
            Expression<Action<TController>> actionSaveGlobal, TestCaseDetailsDTO testCase, UrlHelper urlHelper)
            where TController : Controller
        {
            var widget = new TestCaseWidget(urlHelper);
            widget.SetBrowserName(testCase.TestCase.BrowserName);
            widget.SetTestCaseId(testCase.TestCase.Id);
            widget.SetLocalBlindspots(testCase.TestCase.BlindRegions);
            widget.SetGlobalBlindspots(testCase.GlobalBlindRegions);
            widget.SetSaveLocalBlindspotsAction(actionSaveLocal);
            widget.SetSaveGlobalBlindspotsAction(actionSaveGlobal);
            return widget;
        }
    }
}