using System;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Mvc;

namespace Tellurium.VisualAssertion.Dashboard.Mvc.Widgets
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

        public void SetSource<TController>(Expression<Action<TController>> source, IUrlHelper urlHelper) where TController:Controller
        {
            var url = urlHelper.ActionFor(source);
            SetOption("source", url);
        }

        public static RemoteExpanderWidget Create<TController>(Expression<Action<TController>> source, string targetId, IUrlHelper urlHelper) where TController:Controller
        {
            var widget = new RemoteExpanderWidget();
            widget.SetSource(source, urlHelper);
            widget.SetTargetId(targetId);
            return widget;
        }
    }

}