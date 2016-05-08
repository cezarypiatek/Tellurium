using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Web.Mvc;
using MaintainableSelenium.VisualAssertions.Screenshots.Domain;
using MaintainableSelenium.VisualAssertions.Web.Models.Home;

namespace MaintainableSelenium.VisualAssertions.Web.Mvc
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

        public void SetPatternId(long testCaseId)
        {
            this.SetOption("id", testCaseId);
        } 
        
        public void SetTestCaseId(long testCaseId)
        {
            this.SetOption("caseId", testCaseId);
        }

        public void SetSaveLocalBlindspotsAction<TController>(Expression<Action<TController>> action)
            where TController : Controller
        {
            this.SetOption("actionSaveLocal", urlHelper.ActionFor(action));
        }  
        
        public void SetSaveCategoryBlindspotsAction<TController>(Expression<Action<TController>> action)
            where TController : Controller
        {
            this.SetOption("actionSaveCategory", urlHelper.ActionFor(action));
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
        
        public void SetCategoryBlindspots(List<BlindRegion> blindRegions)
        {
            this.SetComplexOption("categoryRegions", blindRegions);
        }

        public static TestCaseWidget Create<TController>(Expression<Action<TController>> actionSaveLocal,
            Expression<Action<TController>> actionSaveCategory,
            Expression<Action<TController>> actionSaveGlobal, BrowserPatternDTO browserPattern, UrlHelper urlHelper)
            where TController : Controller
        {
            var widget = new TestCaseWidget(urlHelper);
            widget.SetBrowserName(browserPattern.BrowserName);
            widget.SetPatternId(browserPattern.PatternId);
            widget.SetTestCaseId(browserPattern.TestCaseId);
            widget.SetLocalBlindspots(browserPattern.LocalBlindRegions);
            widget.SetCategoryBlindspots(browserPattern.CategoryBlindRegions);
            widget.SetGlobalBlindspots(browserPattern.GlobalBlindRegions);
            widget.SetSaveLocalBlindspotsAction(actionSaveLocal);
            widget.SetSaveCategoryBlindspotsAction(actionSaveCategory);
            widget.SetSaveGlobalBlindspotsAction(actionSaveGlobal);
            return widget;
        }
    }
}