﻿param($installPath, $toolsPath, $package)

function Add-ProjectDirectoryIfNotExist($DirPath)
{	
	$project = Get-Project
    $projectPath = Split-Path $project.FileName -Parent
    $fullPathToNewDire ="$projectPath\$DirPath"
    if((Test-Path $fullPathToNewDire) -ne $true){
        [void](New-Item -ItemType Directory -Force -Path  $fullPathToNewDire)
        $outRoot = ($DirPath -split "\\")[0]
        if([string]::IsNullOrWhiteSpace($outRoot) -ne $true)
        {
            [void]$project.ProjectItems.AddFromDirectory("$projectPath\$outRoot")
        }
    }
    $fullPathToNewDire
}

function Add-FileToProject{
    [CmdletBinding()]
    param([Parameter(ValueFromPipeline=$true)]$Files)
	begin{
		$project = Get-Project
	}
    process{
        foreach($file in $Files)
        {
            $path = if($file -is [System.String]){$file}else{$file.FullName}
            $projectItem = $project.ProjectItems.AddFromFile($path)
            $projectItem.Properties["CopyToOutputDirectory"].Value = 2
        }
    }
}

function New-TempDirectory{
    $tempDirectoryPath = [System.IO.Path]::Combine([System.IO.Path]::GetTempPath(), [System.IO.Path]::GetRandomFileName()) 
    [System.IO.Directory]::CreateDirectory($tempDirectoryPath) | Out-Null  
    $tempDirectoryPath
}

function Get-VersionsFromGoogleapis{
	param($BaseUrl, $DriverName, $Platform="win32")
    $p = Invoke-WebRequest "$BaseUrl/?prefix="
    $o = [xml]$p.Content 
    $o.ListBucketResult.Contents |% {
		$parts =  $_.Key -split "/"; 
		if(($parts.Length -eq 2)  -and ($parts[1].EndsWith(".zip")))
		{
			$versionParts =  $parts[0] -split "\."
			$major = $versionParts[0] -replace "[^\d]",""
			$minor = $versionParts[1] -replace "[^\d]",""
			[PsCustomObject](@{Version= [int]$major*100 +[int]$minor  ; File= "$BaseUrl/$($_.Key)"; VersionString= $parts[0]})
		}
    }|? {$_.File -like "*$Platform*"} | Sort-Object -Property Version
}

function Download-FromGoogleapis{
    param($BaseUrl, $DriverName, $DestinationPath, $Platform="win32")
    $allVersions = Get-VersionsFromGoogleapis -BaseUrl $BaseUrl -DriverName $DriverName -Platform $Platform
	$newestFile = $allVersions | Sort-Object -Property Version | Select-Object -Last 1	
    $tempDir = New-TempDirectory
    $driverTmpPath = "$tempDir\$DriverName.zip"
    Start-BitsTransfer -Source $newestFile.File -Destination $driverTmpPath    
    Expand-Archive -Path $driverTmpPath -DestinationPath $DestinationPath -Force
    Add-FileToProject -Files "$DestinationPath\$DriverName.exe"
    Remove-Item -Path $driverTmpPath -Force -Recurse    
}

function New-DriversDirectory{
    Add-ProjectDirectoryIfNotExist -DirPath "Drivers"
}

function Get-ChromeDriverVersions{
	param([string]$Platform)
	Get-VersionsFromGoogleapis -BaseUrl "http://chromedriver.storage.googleapis.com" -DriverName "chromedriver" -Platform $Platform | Sort-Object -Descending Version | Select-Object VersionString
}

function Install-ChromeDriver{
    param([string]$Platform)
    $driversPath = New-DriversDirectory
    Download-FromGoogleapis -BaseUrl "http://chromedriver.storage.googleapis.com" -DriverName "chromedriver" -Platform $Platform -DestinationPath $driversPath
}

function Get-IEDriverVersions{
	param([string]$Platform)
	Get-VersionsFromGoogleapis -BaseUrl "http://selenium-release.storage.googleapis.com" -DriverName "IEDriverServer" -Platform $Platform | Sort-Object -Descending Version | Select-Object VersionString
}

function Install-IEDriver{
    param([string]$Platform)
    $driversPath = New-DriversDirectory
    Download-FromGoogleapis -BaseUrl "http://selenium-release.storage.googleapis.com" -DriverName "IEDriverServer" -Platform $Platform -DestinationPath  $driversPath
}


function Get-PahntomJSDriverAvailabeFiles{
    $data = Invoke-RestMethod -Method Get -Uri https://api.bitbucket.org/2.0/repositories/ariya/phantomjs/downloads
    $data.values |%{ 
        $nameParts = $_.name -split "-"
        [PsCustomObject]@{name=$nameParts[0]; version=$nameParts[1]; url=$_.links.self.href; platform=$($nameParts[2] -replace "\.zip",""); }
    }|? {$_.platform -eq "windows"} 
}

function Get-PhantomJSDriverVersions{ 
    Get-PahntomJSDriverAvailabeFiles | Sort-Object version -Descending | Select-Object version
}

function Install-PhantomJSDriver{
    $newestPhantom = Get-PahntomJSDriverAvailabeFiles | Sort-Object -Property version -Descending | Select-Object -First 1
    $tmpDir = New-TempDirectory    
    Invoke-RestMethod -Method Get -Uri $newestPhantom.url -OutFile "$tmpDir\phantom.zip"    
    Expand-Archive -Path "$tmpDir\phantom.zip"  -DestinationPath $tmpDir
    $driversPath = New-DriversDirectory
    Get-ChildItem -Filter "phantomjs.exe" -Recurse -Path $tmpDir |  Copy-Item -Destination $driversPath -PassThru | Add-FileToProject
    Remove-Item $tmpDir -Force -Recurse
}

function Get-EdgeDriverAvailableFiles{
    $page = Invoke-WebRequest -Uri https://developer.microsoft.com/en-us/microsoft-edge/tools/webdriver/#downloads
    $page.Links |? {$_.innerText -like "*Release*"} |% { [PsCustomObject]@{version = $_.innerText; path=$_.href } }
}

function Get-EdgeDriverVersions{
    Get-EdgeDriverAvailableFiles | Sort-Object version -Descending | Select-Object version
}

function Install-EdgeDriver{    
    $newestEdge = Get-EdgeDriverAvailableFiles | Sort-Object version -Descending | Select-Object -First 1
    $tmpDir = New-TempDirectory
    Start-BitsTransfer -Source $newestEdge.path -Destination $tmpDir
    $driversPath = New-DriversDirectory
    Get-ChildItem $tmpDir | Copy-Item -Destination $driversPath -PassThru | Add-FileToProject
    Remove-Item $tmpDir -Force -Recurse   
}

function Get-OperaDriverAvailableFiles{
     $relases = Invoke-RestMethod -Method Get -Uri https://api.github.com/repos/operasoftware/operachromiumdriver/releases
     foreach($release in $relases)     
     {        
        $version = $release.name
        foreach($asset in $release.assets)
        {            
            $nameParts = $asset.name -split "[_\.]"
            if($nameParts.length -eq 3)
            {
                [pscustomobject](@{version = $version; platform=$nameParts[1]; url=$asset.browser_download_url })
            }
        }
    }
}

function Get-OperaDriverVersions{
    Get-OperaDriverAvailableFiles | Select-Object version
}

function Install-OperaDriver{
    param([string]$Platform)    
    $windowsEdition = Get-OperaDriverAvailableFiles |? {$_.platform -like "*$Platform*"} | Select-Object -First 1
    $tmpDir = New-TempDirectory
    Invoke-RestMethod -Method Get -Uri $windowsEdition.browser_download_url -OutFile "$tmpDir\opera.zip"
    Expand-Archive -Path "$tmpDir\opera.zip" -DestinationPath $tmpDir
    $driversPath = New-DriversDirectory
    Copy-Item "$tmpDir\operadriver.exe" -Destination $driversPath -PassThru | Add-FileToProject
    Remove-Item -Path $tmpDir -Force -Recurse
}

function Install-SeleniumWebDriver{
    [CmdletBinding()]
    param(
    [Parameter(Mandatory=$true)][ValidateSet("Chrome","PhantomJs","InternetExplorer","Edge","Firefox", "Opera")][string]$Browser,
    [ValidateSet("win32","win64")]$Platform="win32"
    )	
    switch($Browser)
    {
        "Chrome" {Install-ChromeDriver -Platform $Platform; break}
        "PhantomJs" {Install-PhantomJSDriver; break}
        "InternetExplorer" {Install-IEDriver -Platform $Platform; break}
        "Edge" {Install-EdgeDriver; break}
        "Firefox" {Write-Host "No need to download anything. Selenium support Firefox out of the box."; break}
        "Opera" {Install-OperaDriver -Platform $Platform; break}
        default {"Unsupported browser type. Please select browser from the follwing list: Chrome, PhantomJs, InternetExplorer, Edge, Firefox, Opera"}    
    }
}

function Get-SeleniumWebDriverVersions{
	[CmdletBinding()]
    param(
    [Parameter(Mandatory=$true)][ValidateSet("Chrome","PhantomJs","InternetExplorer","Edge","Firefox", "Opera")][string]$Browser,
    [ValidateSet("win32","win64")]$Platform="win32"
    )
	
	switch($Browser)
    {
        "Chrome" {Get-ChromeDriverVersions -Platform $Platform; break}
        "PhantomJs" {Get-PhantomJSDriverVersions; break}
        "InternetExplorer" {Get-IEDriverVersions -Platform $Platform; break}
        "Edge" {Get-EdgeDriverVersions; break}
        "Firefox" {Write-Host "No need to download anything. Selenium support Firefox out of the box."; break}
        "Opera" {Get-OperaDriverVersions -Platform $Platform; break}
        default {"Unsupported browser type. Please select browser from the follwing list: Chrome, PhantomJs, InternetExplorer, Edge, Firefox, Opera"}    
    }
}

Export-ModuleMember -Function Install-SeleniumWebDriver, Get-SeleniumWebDriverVersions