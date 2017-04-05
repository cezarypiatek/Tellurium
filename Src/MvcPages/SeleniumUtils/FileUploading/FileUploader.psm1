$ErrorActionPreference = "Stop"

function Get-BrowserProcessName($BrowserName)
{
    switch($BrowserName){
        "Firefox" { "Firefox"}
        "Chrome" { "chrome"}
        "InternetExplorer" { "iexplore"}
        "Opera" { "opera"}
        default { throw "Not supported browser" }
    }
}

function Get-UploadWindow($BrowserName, $TimeOut)
{    
    $browserProcessName = Get-BrowserProcessName -BrowserName $BrowserName
    $windows = Select-Window -ProcessName $browserProcessName
    $children = Select-ChildWindow -Window $windows
    $uploadWindow = $children | Select-Object -First 1
    if($uploadWindow -ne $null)
    {
        $uploadWindow
    }elseif($TimeOut -eq 0)
    {
        throw "Cannot find upload window"
    }
    else{
        Start-Sleep -Seconds 1
        Get-UploadWindow -BrowserName $BrowserName -TimeOut ($TimeOut-1)
    }
}

$definition = @"
    using System.Runtime.InteropServices;
    using System;
    public static class Robot
    {
        private static int WM_SETTEXT = 0x0C;
        private static int WM_LBUTTONDOWN = 0x201;
        private static int WM_LBUTTONUP = 0x202;

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
    }
    
"@
Add-Type -TypeDefinition $definition

function Upload-File($BrowserName, $FilePath, $TimeOut=30)
{
    $uploadWindow = Get-UploadWindow -BrowserName $BrowserName -TimeOut $TimeOut    
    $uploadWindow.Activate()
    $fileNameInput = $uploadWindow | Select-Control -Recurse -Class "Edit" | Select-Object -First 1  
    $fileNameInput.Activate()
    [Robot]::SendText($fileNameInput,"$FilePath")
    $acceptButton =  $uploadWindow | Select-Control -Class "Button" | Select-Object -First 1
    $acceptButton.Activate()
    Start-Sleep -Seconds 1
    [Robot]::SendClick($acceptButton)
}