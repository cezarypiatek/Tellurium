Import-Module .\WASP.dll
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

function Upload-File($BrowserName, $FilePath, $TimeOut=30)
{
    $uploadWindow = Get-UploadWindow -BrowserName $BrowserName -TimeOut $TimeOut    
    $fileNameInput = $uploadWindow | Select-Control -Recurse -Class "Edit" | Select-Object -First 1  
    $fileNameInput.Activate()
    $fileNameInput | Send-Keys "$FilePath{enter}"
}