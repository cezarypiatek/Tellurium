using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Tellurium.MvcPages.SeleniumUtils.FileUploading.WindowsInternals
{
    internal static class WindowsMethods
    {
        public delegate bool EnumWindowsCallback(IntPtr hWnd, int param);

        public static IntPtr GetWindowLongish(IntPtr hWnd, int nIndex)
        {
            if (IntPtr.Size == 8)
                return GetWindowLongPtr(hWnd, nIndex);
            return new IntPtr(GetWindowLong(hWnd, nIndex));
        }

        [DllImport("User32")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool EnumChildWindows(IntPtr hWnd, EnumWindowsCallback callback, int param);

        [DllImport("User32")]
        public static extern bool EnumWindows(EnumWindowsCallback callback, int param);

        [DllImport("User32")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetForegroundWindow(IntPtr hWnd);
        
        [DllImport("User32")]
        public static extern int GetWindowThreadProcessId(IntPtr window, ref int processId);

        [DllImport("User32")]
        public static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

        [DllImport("User32")]
        public static extern bool IsWindow(IntPtr hWnd);

        [DllImport("User32")]
        public static extern bool IsWindowVisible(IntPtr hWnd);
        
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("User32", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool SendMessageTimeout(IntPtr windowHandle, int Msg, IntPtr wParam, IntPtr lParam, int flags, uint timeout, out IntPtr result);

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("User32", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool SendMessageTimeout(IntPtr windowHandle, int Msg, IntPtr wParam, StringBuilder lParam, int flags, uint timeout, out IntPtr result); 

        public static string GetWindowCaption(IntPtr hWnd)
        {
            int stringLength = 0;
            IntPtr length;
            uint timeout = 500;
            if (stringLength == 0)
                if (SendMessageTimeout(hWnd, WM_GetTextLength, IntPtr.Zero, IntPtr.Zero, SMTO_AbortIfHung, timeout, out length))
                    stringLength = length.ToInt32() + 1;
                else
                    return string.Empty;

            var result = new StringBuilder(stringLength);

            if (SendMessageTimeout(hWnd, WM_GetText, new IntPtr(stringLength), result, SMTO_AbortIfHung, timeout, out length))
                return result.ToString();
            return string.Empty;
        }

        private const int SMTO_AbortIfHung = 0x0002;
        private const int WM_SETTEXT = 0x0C;
        private const int WM_LBUTTONDOWN = 0x201;
        private const int WM_LBUTTONUP = 0x202;
        private const int WM_GetText = 0x00D;
        private const int WM_GetTextLength = 0x00E;

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hwnd, int msg, IntPtr wParam, [MarshalAs(UnmanagedType.LPStr)] string lParam);

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int msg, uint wParam, uint lParam);


        public static void SendClick(IntPtr buttonHandle)
        {
            SendMessage(buttonHandle, WM_LBUTTONDOWN, 0, 0);
            SendMessage(buttonHandle, WM_LBUTTONUP, 0, 0);
            SendMessage(buttonHandle, WM_LBUTTONDOWN, 0, 0);
            SendMessage(buttonHandle, WM_LBUTTONUP, 0, 0);
        }

        public static void SendText(System.IntPtr textBoxHandle, string text)
        {
            SendMessage(textBoxHandle, WM_SETTEXT, (System.IntPtr)text.Length, text);
        }

        public static string GetClassName(IntPtr hWnd)
        {
            var sb = new StringBuilder(100);
            GetClassName(hWnd, sb, sb.Capacity);
            return sb.ToString();
        }

        [DllImport("User32")]
        public static extern IntPtr GetParent(IntPtr window);

        [DllImport("User32")]
        public static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

        [DllImport("User32", EntryPoint = "GetWindowLong")]
        public static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("User32", EntryPoint = "GetWindowLongPtr")]
        public static extern IntPtr GetWindowLongPtr(IntPtr hWnd, int nIndex);

        [DllImport("User32")]
        public static extern IntPtr GetWindow(IntPtr hWnd, int uCmd);

        public const int GW_Owner = 4;

        public const int GWL_HWndParent = -8;
        public const int GWL_Style = -16;
        public const int GWL_ExStyle = -20;

        public const int WS_Child = 0x40000000;

        public const int WSEX_ApplicationWindow = 0x00040000;
        public const int WSEX_ToolWindow = 0x00000080;
    }
}