param($installPath, $toolsPath, $package, $project)

function Add-ProjectDirectoryIfNotExist($Project, $DirPath)
{
    $projectPath = Split-Path $Project.FileName -Parent
    $fullPathToNewDire ="$projectPath\$DirPath"
    if((Test-Path $fullPathToNewDire) -ne $true){
        [void](New-Item -ItemType Directory -Force -Path  $fullPathToNewDire)
        $outRoot = ($DirPath -split "\\")[0]
        if([string]::IsNullOrWhiteSpace($outRoot) -ne $true)
        {
            [void]$Project.ProjectItems.AddFromDirectory("$projectPath\$outRoot")
        }
    }
    $fullPathToNewDire
}

function Add-FileToProject{
    [CmdletBinding()]
    param([Parameter(ValueFromPipeline=$true)]$Files)
    process{
        foreach($file in $Files)
        {
            $path = if($file -is [System.String]){$file}else{$file.FullName}
            $projectItem = $Project.ProjectItems.AddFromFile($path)
            $projectItem.Properties["CopyToOutputDirectory"].Value = 2
        }
    }
}

function New-TempDirectory{
    $tempDirectoryPath = [System.IO.Path]::Combine([System.IO.Path]::GetTempPath(), [System.IO.Path]::GetRandomFileName()) 
    [System.IO.Directory]::CreateDirectory($tempDirectoryPath) | Out-Null  
    $tempDirectoryPath
}

function Download-FromGoogleapis{
    param($BaseUrl, $DriverName, $DestinationPath)
    $platform = "win32"     
    $p = Invoke-WebRequest "$BaseUrl/?prefix="
    $o = [xml]$p.Content 
    $newestFile = $o.ListBucketResult.Contents |% {
    $parts =  $_.Key -split "/"; 
    if(($parts.Length -eq2)  -and($parts[1].EndsWith(".zip")))
    {
        $versionParts =  $parts[0] -split "\."
        $major = $versionParts[0] -replace "[^\d]",""
        $minor = $versionParts[1] -replace "[^\d]",""
        [PsCustomObject](@{Version= [int]$major*100 +[int]$minor  ; File= "$BaseUrl/$($_.Key)"})
    }
    }|? {$_.File -like "*$platform*"} | Sort-Object -Property Version | Select-Object -Last 1
    $tempDir = New-TempDirectory
    $driverTmpPath = "$tempDir\$DriverName.zip"
    Start-BitsTransfer -Source $newestFile.File -Destination $driverTmpPath    
    Expand-Archive -Path $driverTmpPath -DestinationPath $DestinationPath -Force
    Add-FileToProject -Files "$DestinationPath\$DriverName.exe"
    Remove-Item -Path $driverTmpPath -Force -Recurse    
}

function New-DriversDirectory{
    Add-ProjectDirectoryIfNotExist -Project $project -DirPath "Drivers"
}

function Install-ChromeDriver{
    $driversPath = New-DriversDirectory
    Download-FromGoogleapis -BaseUrl "http://chromedriver.storage.googleapis.com" -DriverName "chromedriver" -DestinationPath $driversPath
}

function Install-IEDriver{
    $driversPath = New-DriversDirectory
    Download-FromGoogleapis -BaseUrl "http://selenium-release.storage.googleapis.com" -DriverName "IEDriverServer" -DestinationPath  $driversPath
}


function Install-PhantomJSDriver{    
    $data = Invoke-RestMethod -Method Get -Uri https://api.bitbucket.org/2.0/repositories/ariya/phantomjs/downloads
    $newestPhantom = $data.values |%{ 
        $nameParts = $_.name -split "-"
        @{name=$nameParts[0]; version=$nameParts[1]; url=$_.links.self.href; platform=$($nameParts[2] -replace "\.zip",""); }
    }|? {$_.platform -eq "windows"}  | Sort-Object -Property versionstamp -Descending | Select-Object -First 1
    $tmpDir = New-TempDirectory    
    Invoke-RestMethod -Method Get -Uri $newestPhantom.url -OutFile "$tmpDir\phantom.zip"    
    Expand-Archive -Path "$tmpDir\phantom.zip"  -DestinationPath $tmpDir
    $driversPath = New-DriversDirectory
    Get-ChildItem -Filter "phantomjs.exe" -Recurse -Path $tmpDir |  Copy-Item -Destination $driversPath -PassThru | Add-FileToProject
    Remove-Item $tmpDir -Force -Recurse
}

function Install-EdgeDriver{
    $page = Invoke-WebRequest -Uri https://developer.microsoft.com/en-us/microsoft-edge/tools/webdriver/#downloads
    $newestEdge = $page.Links |? {$_.innerText -like "*Release*"} | Sort-Object -Property innerText -Descending | Select-Object -First 1
    $tmpDir = New-TempDirectory
    Start-BitsTransfer -Source $newestEdge.href -Destination $tmpDir
    $driversPath = New-DriversDirectory
    Get-ChildItem $tmpDir | Copy-Item -Destination $driversPath -PassThru | Add-FileToProject
    Remove-Item $tmpDir -Force -Recurse   
}

function Install-OperaDriver{
    $data = Invoke-RestMethod -Method Get -Uri https://api.github.com/repos/operasoftware/operachromiumdriver/releases/latest
    $windowsEdition = $data.assets |? {$_.name -like "*win32*"} | Select-Object -First 1
    $tmpDir = New-TempDirectory
    Invoke-RestMethod -Method Get -Uri $windowsEdition.browser_download_url -OutFile "$tmpDir\opera.zip"
    Expand-Archive -Path "$tmpDir\opera.zip" -DestinationPath $tmpDir
    $driversPath = New-DriversDirectory
    Copy-Item "$tmpDir\operadriver.exe" -Destination $driversPath -PassThru | Add-FileToProject
    Remove-Item -Path $tmpDir -Force -Recurse
}

function Install-SeleniumWebDriver{
    [CmdletBinding()]
    param([Parameter(Mandatory=$true)][ValidateSet("Chrome","PhantomJs","InternetExplorer","Edge","Firefox", "Opera")][string]$Browser)
    switch($Browser)
    {
        "Chrome" {Install-ChromeDriver; break}
        "PhantomJs" {Install-PhantomJSDriver; break}
        "InternetExplorer" {Install-IEDriver; break}
        "Edge" {Install-EdgeDriver; break}
        "Firefox" {Write-Host "No need to download anything. Selenium support Firefox out of the box."; break}
        "Opera" {Install-OperaDriver; break}
        default {"Unsupported browser type. Please select browser from the follwing list: Chrome, PhantomJs, InternetExplorer, Edge, Firefox, Opera"}    
    }
}

Export-ModuleMember -Function Install-SeleniumWebDriver