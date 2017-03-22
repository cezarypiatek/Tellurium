using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.Routing;

namespace Tellurium.VisualAssertion.Dashboard.Mvc.TagHelpers
{

    [HtmlTargetElement("a", Attributes = "active-link")]
    public class ActiveLink : TagHelper
    {
        private readonly IHttpContextAccessor httpContextAccessor;

        public ActiveLink(IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
        }

        public string ActiveLinkWrapper { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (ShouldWrappElement())
                HandleWrappedElement(context, output);
            else
                HandleNonWrappElement(context, output);


            output.Attributes.RemoveAll("active-link");
        }

        private void HandleNonWrappElement(TagHelperContext context, TagHelperOutput output)
        {
            if (IsActive(context))
            {
                output.Attributes.SetAttribute("class", "active");
            }
        }

        private void HandleWrappedElement(TagHelperContext context, TagHelperOutput output)
        {
            if (IsActive(context))
            {
                output.PreElement.SetHtmlContent($"<{ActiveLinkWrapper} class=\"active\">");
            }
            else
            {
                output.PreElement.SetHtmlContent($"<{ActiveLinkWrapper}>");
            }
            output.PostElement.SetHtmlContent($"</{ActiveLinkWrapper}>");
        }

        private bool ShouldWrappElement()
        {
            return string.IsNullOrWhiteSpace(ActiveLinkWrapper)==false;
        }

        private bool IsActive(TagHelperContext context)
        {
            return GetControllerName(context) == GetCurrentControllerName() && GetActionName(context) == GetCurrentActionName();
        }

        private static string GetActionName(TagHelperContext context)
        {
            return context.AllAttributes["asp-action"].Value.ToString();
        }

        private static string GetControllerName(TagHelperContext context)
        {
            return context.AllAttributes["asp-controller"].Value.ToString();
        }

        private string GetCurrentActionName()
        {
            return httpContextAccessor.HttpContext.GetRouteData().Values["action"] as string;
        }

        private string GetCurrentControllerName()
        {
            return httpContextAccessor.HttpContext.GetRouteData().Values["controller"] as string;
        }
    }
}