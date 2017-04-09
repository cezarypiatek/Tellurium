using System;
using System.Collections.Generic;

namespace Tellurium.MvcPages.SeleniumUtils.FileUploading.WindowsInternals
{
    internal class ControlHandle
    {
        public ControlHandle(IntPtr handle)
        {
            Handle = handle;
            Title = WindowsMethods.GetWindowCaption(handle);
            Class = WindowsMethods.GetClassName(handle);
        }

        public IntPtr Handle { get; }

        public string Title { get; }

        public string Class { get; }

        public IEnumerable<ControlHandle> GetControls(string controlClass)
        {
            foreach (var found in GetAllControls())
                if (controlClass == found.GetClassName())
                    yield return new ControlHandle(found);
        }

        private List<ControlHandle> GetAllControls()
        {
            var controlsList = new List<ControlHandle>();
            WindowsMethods.EnumChildWindows(Handle, (hWnd, param) =>
            {
                controlsList.Add(hWnd);
                return true;
            }, 0);
            return controlsList;
        }

        public IEnumerable<ControlHandle> GetChildrenWindows()
        {
            ControlHandle found = WindowsMethods.FindWindowEx(IntPtr.Zero, IntPtr.Zero, null, null);
            while (found.Equals(IntPtr.Zero) == false)
            {
                if (found.IsVisibleWindow() && found.GetParentOrOwner() == Handle)
                    yield return found;
                found = WindowsMethods.FindWindowEx(IntPtr.Zero, found, null, null);
            }
        }

        public bool IsAppWindow()
        {
            if (IsVisibleWindow())
            {
                var stylex = WindowsMethods.GetWindowLong(Handle, WindowsMethods.GWL_ExStyle);
                if (WindowsMethods.WSEX_ApplicationWindow != (stylex & WindowsMethods.WSEX_ApplicationWindow))
                {
                    var style = WindowsMethods.GetWindowLong(Handle, WindowsMethods.GWL_Style);
                    if (IntPtr.Zero.Equals(WindowsMethods.GetWindowLongish(Handle, WindowsMethods.GWL_HWndParent)) &&
                        IntPtr.Zero.Equals(WindowsMethods.GetWindow(Handle, WindowsMethods.GW_Owner)) &&
                        WindowsMethods.WS_Child != (style & WindowsMethods.WS_Child))
                    {
                        if (WindowsMethods.WSEX_ToolWindow == (stylex & WindowsMethods.WSEX_ToolWindow))
                            return false;
                    }
                    else
                    {
                        return false;
                    }
                }
                var windowClass = WindowsMethods.GetClassName(Handle);

                if (windowClass.Equals("WindowsScreensaverClass") || windowClass.Equals("tooltips_class32"))
                    return false;
            }
            else
            {
                return false;
            }
            return true;
        }

        private bool IsVisibleWindow()
        {
            return WindowsMethods.IsWindow(Handle) && WindowsMethods.IsWindowVisible(Handle);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as ControlHandle);
        }

        private bool Equals(ControlHandle wnd)
        {
            return Handle == wnd.Handle && Class == wnd.Class && Title == wnd.Title;
        }

        public override int GetHashCode()
        {
            return Handle.GetHashCode();
        }

        private string GetClassName()
        {
            return WindowsMethods.GetClassName(this);
        }

        private IntPtr GetParentOrOwner()
        {
            return WindowsMethods.GetParent(Handle);
        }

        public void Activate()
        {
            WindowsMethods.SetForegroundWindow(this);
        }

        public static implicit operator IntPtr(ControlHandle wnd)
        {
            return wnd.Handle;
        }

        public static implicit operator ControlHandle(IntPtr value)
        {
            return new ControlHandle(value);
        }

        public static implicit operator int(ControlHandle wnd)
        {
            return wnd.Handle.ToInt32();
        }

        public static implicit operator ControlHandle(int value)
        {
            return new ControlHandle((IntPtr) value);
        }
    }
}