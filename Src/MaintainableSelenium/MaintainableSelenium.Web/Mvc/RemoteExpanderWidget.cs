using System;
using System.Linq.Expressions;
using System.Web.Mvc;

namespace MaintainableSelenium.Web.Mvc
{
    public class RemoteExpanderWidget:JqueryUIWidget
    {
        public RemoteExpanderWidget() : base("remoteexpander")
        {
        }

        public void SetTargetId(string targetId)
        {
            SetOption("targetid", targetId);
        }

        public void SetSource<TController>(Expression<Action<TController>> source, UrlHelper urlHelper) where TController:Controller
        {
            var url = urlHelper.ActionFor(source);
            SetOption("source", url);
        }

        public static RemoteExpanderWidget Create<TController>(Expression<Action<TController>> source, string targetId, UrlHelper urlHelper) where TController:Controller
        {
            var widget = new RemoteExpanderWidget();
            widget.SetSource(source, urlHelper);
            widget.SetTargetId(targetId);
            return widget;
        }
    }

}