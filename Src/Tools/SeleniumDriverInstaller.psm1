function New-ParameterSet {
    param([scriptblock]$Params)
    $runtimeParameterDictionary = New-Object System.Management.Automation.RuntimeDefinedParameterDictionary
    $parameters = . $Params
    foreach ($parameter in $parameters) {
        $runtimeParameterDictionary.Add($parameter.Name, $parameter)
    }
    $runtimeParameterDictionary
}

function New-Parameter {
    param($Name, $Position, $ValidateSet, $Mandatory = $false)
    $attributeCollection = New-Object System.Collections.ObjectModel.Collection[System.Attribute]
    $parameterAttribute = New-Object System.Management.Automation.ParameterAttribute
    $parameterAttribute.Mandatory = $Mandatory
    $parameterAttribute.Position = $Position
    $attributeCollection.Add($parameterAttribute)
    $validateSetAttribute = New-Object System.Management.Automation.ValidateSetAttribute -ArgumentList $ValidateSet
    $attributeCollection.Add($validateSetAttribute)
    New-Object System.Management.Automation.RuntimeDefinedParameter($Name, [string], $attributeCollection)
}

function Find-Matches {
    [CmdletBinding()]
    param(
        [Parameter(ValueFromPipeline = $true, Mandatory = $true)][string]$Text,
        [string]$Pattern
    )
    process {
        foreach ($t in $Text) {
            [regex]::Matches($t, $Pattern) | Where-Object {$_.Length -gt 0}
        }
    }
}

function Test-VSContext {
    [bool] (Get-Command "Get-Project" -ErrorAction SilentlyContinue)
}
function Add-ProjectDirectoryIfNotExist($DirPath) {
    $project = Get-Project
    $projectPath = Split-Path $project.FileName -Parent
    $fullPathToNewDirectory = "$projectPath\$DirPath"
    if ((Test-Path $fullPathToNewDirectory) -ne $true) {
        [void](New-Item -ItemType Directory -Force -Path  $fullPathToNewDirectory)
        $outRoot = ($DirPath -split "\\")[0]
        if ([string]::IsNullOrWhiteSpace($outRoot) -ne $true) {
            [void]$project.ProjectItems.AddFromDirectory("$projectPath\$outRoot")
        }
    }
    $fullPathToNewDirectory
}

function Add-FileToProject {
    [CmdletBinding()]
    param([Parameter(ValueFromPipeline = $true)]$Files)
    begin {
        if (Test-VSContext) {
            $project = Get-Project
        }

    }
    process {
        foreach ($file in $Files) {
            if ($project -eq $null) {
                break
            }
            $path = if ($file -is [System.String]) {$file}else {$file.FullName}
            $projectItem = $project.ProjectItems.AddFromFile($path)
            $projectItem.Properties["CopyToOutputDirectory"].Value = 2
        }
    }
}
function New-DriversDirectory {
    param([string]$Directory = "Drivers")
    if ([string]::IsNullOrWhiteSpace($Directory)) {
        $Directory = "Drivers"
    }
    if (Test-VSContext) {
        Add-ProjectDirectoryIfNotExist -DirPath $Directory
    }
    else {
        if ((Test-Path $Directory) -eq $false) {
            New-Item -Name $Directory -ItemType Directory -Force | Out-Null
        }
        Get-Item $Directory | Select-Object -ExpandProperty FullName
    }
}
function Get-SelectedOrDefault {
    param([string]$Selected, [string]$Default)
    if ([string]::IsNullOrWhiteSpace($Selected)) {
        $Default
    }
    else {
        $Selected
    }
}

function New-TempDirectory {
    $tempDirectoryPath = [System.IO.Path]::Combine([System.IO.Path]::GetTempPath(), [System.IO.Path]::GetRandomFileName())
    [System.IO.Directory]::CreateDirectory($tempDirectoryPath) | Out-Null
    $tempDirectoryPath
}

function Get-InstallerConfigFilePath{
    $currentPath = if(Test-VSContext){
        $project = Get-Project
        Split-Path $project.FullName -Parent
    }else{
        Get-Location
    }
    "$currentPath\SeleniumWebdrivers.json"
}
function  Save-InstalledDriverInfo {
    param($Browser, $DriverVersionInfo, $OutputDir, $DriverFileName)
    $configFile =  Get-InstallerConfigFilePath
    $config = if(Test-Path $configFile)
                {
                    Get-Content $configFile -Raw | ConvertFrom-Json
                }else{
                    @{drivers= @(); autoRestore=$true}
                }

    $driverDetails = @{
        browser= $Browser;
        version = $DriverVersionInfo.Version;
        platform = $DriverVersionInfo.Platform;
        outputDir = $OutputDir | Resolve-Path -Relative;
        file = $DriverFileName
    }
    $wasDriverDetailsFound = $false
    $isInRestoreMode = $false
    for($i=0; $i -lt $config.drivers.length; $i++ )
    {
        if($config.drivers[$i].browser -eq $Browser)
        {
            if(($config.drivers[$i].version -eq $driverDetails.version) -and ($config.drivers[$i].platform -eq $driverDetails.platform) -and ($config.drivers[$i].outputDir -eq $driverDetails.outputDir))
            {
                $isInRestoreMode = $true
            }
            $config.drivers[$i] = $driverDetails
            $wasDriverDetailsFound = $true
            break
        }
    }

    if($wasDriverDetailsFound -eq $false)
    {
        $config.drivers+=$driverDetails
    }

    if($isInRestoreMode -eq $false){
        $config | ConvertTo-Json | Out-File -FilePath $configFile
        $configFile | Add-FileToProject
    }
}


function Select-DriverVersion {
    param($AvailableDrivers, $Version)
    $selectedVersion = if ([string]::IsNullOrWhiteSpace($Version)) {
        $AvailableDrivers | Sort-Object -Property VersionNumber, Version | Select-Object -Last 1 -ExpandProperty Version
    }
    else {
        $Version
    }
    $AvailableDrivers | Where-Object {$_.Version -eq $selectedVersion} | Select-Object -First 1
}

function Get-VersionNumber{
    param($VersionString)
    $parts =   $VersionString -split "\." |ForEach-Object {$_ -replace "[^\d]", ""}
    $versionNumber = 0;
    for ($i = 0; $i -lt $parts.Count; $i++) {
        $versionNumber+= [math]::Pow(10,$parts.Count-$i+3) *([int]$parts[$i])
    }
    $versionNumber 
}
function Get-VersionsFromGoogleapis {
    param($BaseUrl, $DriverName, $Platform)
    $p = Invoke-WebRequest "$BaseUrl/?prefix=" -Headers @{"Accept-Encoding" = "gzip"}
    $o = [xml]$p.Content
    ($o.ListBucketResult.Contents) |? { $_.Key -like "*$DriverName*" }  | % {
        $parts = $_.Key -split "/";
        if (($parts.Length -eq 2) -and ($parts[1].EndsWith(".zip"))) {
            $versionNumber = Get-VersionNumber -VersionString $parts[0]
            $elementPlatform = ($parts[1] -split "[_\.]")[1]
            [PsCustomObject](@{VersionNumber = $versionNumber ; File = "$BaseUrl/$($_.Key)"; Version = $parts[0]; Platform = $elementPlatform} )
        }
    } | Where-Object { ([string]::IsNullOrWhiteSpace($Platform) -eq $true) -or ($_.Platform -eq "$Platform")} | Sort-Object -Property VersionNumber
}

function Get-FromGoogleapis {
    [CmdletBinding()]
    param($Browser, $BaseUrl, $DriverName, $DestinationPath, $Platform, $Version)
    $allVersions = Get-VersionsFromGoogleapis -BaseUrl $BaseUrl -DriverName $DriverName -Platform $Platform
    $newestFile = Select-DriverVersion -AvailableDrivers $allVersions -Version $Version
    Write-Verbose "Install version '$($newestFile.Version)' for platform '$($newestFile.Platform)'"
    $tempDir = New-TempDirectory
    $driverTmpPath = "$tempDir\$DriverName.zip"
    Start-BitsTransfer -Source $newestFile.File -Destination $driverTmpPath
    Expand-Archive -Path $driverTmpPath -DestinationPath $DestinationPath -Force
    Add-FileToProject -Files "$DestinationPath\$DriverName.exe"
    Remove-Item -Path $driverTmpPath -Force -Recurse
    Save-InstalledDriverInfo -Browser $Browser -DriverVersionInfo $newestFile -OutputDir $DestinationPath -DriverFileName "$DriverName.exe"
}

function Get-ChromeDriverVersions {
    [CmdletBinding()]
    param([string]$Platform)
    Get-VersionsFromGoogleapis -BaseUrl "http://chromedriver.storage.googleapis.com" -DriverName "chromedriver" -Platform $Platform | Sort-Object -Descending VersionNumber, Platform | Select-Object -Property @{n = "Driver"; e = {"Chrome"}}, Version, Platform
}

function Install-ChromeDriver {
    [CmdletBinding()]
    param(
        [string]$Platform = "win32",
        [string]$OutputDir
    )
    DynamicParam {
        New-ParameterSet -Params {
            $availableVersions = Get-ChromeDriverVersions -Platform $Platform | Select-Object -ExpandProperty Version
            New-Parameter -Name "Version" -Position 1 -ValidateSet $availableVersions
        }
    }
    process {
        $version = $PsBoundParameters["Version"]
        $Platform = Get-SelectedOrDefault -Selected $Platform -Default "win32"
        Get-FromGoogleapis -Browser "Chrome" -BaseUrl "http://chromedriver.storage.googleapis.com" -DriverName "chromedriver" -Platform $Platform -Version $version -DestinationPath $OutputDir
    }
}

function Get-IEDriverVersions {
    param([string]$Platform)
    Get-VersionsFromGoogleapis -BaseUrl "http://selenium-release.storage.googleapis.com" -DriverName "IEDriverServer" -Platform $Platform | Sort-Object -Descending VersionNumber | Select-Object -Property @{n = "Driver"; e = {"InternetExplorer"}}, Version, Platform
}

function Install-IEDriver {
    [CmdletBinding()]
    param(
        [string]$Platform = "Win32",
        [string]$OutputDir
    )
    DynamicParam {
        New-ParameterSet -Params {
            $availableVersions = Get-IEDriverVersions -Platform $Platform | Select-Object -ExpandProperty Version
            New-Parameter -Name "Version" -Position 1 -ValidateSet $availableVersions
        }
    }
    process {
        $version = $PsBoundParameters["Version"]
        $Platform = Get-SelectedOrDefault -Selected $Platform -Default "Win32"
        Get-FromGoogleapis -Browser "InternetExplorer" -BaseUrl "http://selenium-release.storage.googleapis.com" -DriverName "IEDriverServer" -Platform $Platform -Version $version -DestinationPath  $OutputDir
    }
}


function Get-PahntomJSDriverAvailabeFiles {
    param([string]$Platform)
    $data = Invoke-RestMethod -Method Get -Uri https://api.bitbucket.org/2.0/repositories/ariya/phantomjs/downloads
    foreach ($item in $data.values) {
        if ($item.name -match "phantomjs-([\d\.]+)-(.*?)\.(.*)") {
            $filePlatform = $Matches[2]
            if (([String]::IsNullOrWhiteSpace($Platform) -ne $true) -and ($Platform -ne $fileplatform)) {
                continue
            }
            [PsCustomObject]@{Version = $Matches[1]; Url = $item.links.self.href; Platform = $filePlatform  ; }
        }
    }
}

function Get-PhantomJSDriverVersions {
    param([string]$Platform)
    Get-PahntomJSDriverAvailabeFiles -Platform $Platform| Sort-Object Version -Descending | Select-Object -Property @{n = "Driver"; e = {"Phantom"}}, Version, Platform
}

function Install-PhantomJSDriver {
    [CmdletBinding()]
    param(
        [string]$Platform = "beta-windows",
        [string]$OutputDir
    )
    DynamicParam {
        New-ParameterSet -Params {
            $availableVersions = Get-PhantomJSDriverVersions -Platform $Platform | Select-Object -ExpandProperty Version
            New-Parameter -Name "Version" -Position 1 -ValidateSet $availableVersions
        }
    }
    process {
        $Platform = Get-SelectedOrDefault -Selected $Platform -Default "beta-windows"
        $version = $PsBoundParameters["Version"]
        $allVersions = Get-PahntomJSDriverAvailabeFiles -Platform $Platform
        $newestPhantom = Select-DriverVersion -AvailableDrivers $allVersions -Version $version
        Write-Verbose "Install version '$($newestPhantom.Version)' for platform '$($newestPhantom.Platform)'"
        $tmpDir = New-TempDirectory
        Invoke-RestMethod -Method Get -Uri $newestPhantom.Url -OutFile "$tmpDir\phantom.zip"
        Expand-Archive -Path "$tmpDir\phantom.zip"  -DestinationPath $tmpDir
        Get-ChildItem -Filter "phantomjs.exe" -Recurse -Path $tmpDir |  Copy-Item -Destination $OutputDir -PassThru | Add-FileToProject
        Remove-Item $tmpDir -Force -Recurse
        Save-InstalledDriverInfo -Browser "PhantomJs" -DriverVersionInfo $newestPhantom -OutputDir $OutputDir -DriverFileName "phantomjs.exe"
    }
}

function Get-EdgeDriverAvailableFiles {
    $page = Invoke-WebRequest -Uri https://developer.microsoft.com/en-us/microsoft-edge/tools/webdriver/#downloads
    $versions = $page.Content | Find-Matches -Pattern "Version: (.*?) \| Edge version supported: (.*?) \|" |  ForEach-Object { @{Driver = $_.Groups[1].Value; Edge = $_.Groups[2].Value} }
    $page.Links |Where-Object {$_.innerText -like "*Release*"} |ForEach-Object {
        foreach ($v in $versions) {
            $releaseVersion = $_.innerText -replace "Release ", ""
            if ($v.Edge -like "*$($releaseVersion)*" ) {
                [PsCustomObject]@{version = $v.Driver; path = $_.href; platform = "windows" }
            }
        }
    }
}

function Get-EdgeDriverVersions {
    Get-EdgeDriverAvailableFiles | Sort-Object version -Descending | Select-Object -Property @{n = "Driver"; e = {"Edge"}}, version, platform
}

function Install-EdgeDriver {
    [CmdletBinding()]
    param(
        [string]$Platform,
        [string]$OutputDir
    )
    DynamicParam {
        New-ParameterSet -Params {
            $availableVersions = Get-EdgeDriverVersions | Select-Object -ExpandProperty Version
            New-Parameter -Name "Version" -Position 1 -ValidateSet $availableVersions
        }
    }
    process {
        $version = $PsBoundParameters["Version"]
        $allVersions = Get-EdgeDriverAvailableFiles
        $newestEdge = Select-DriverVersion -AvailableDrivers $allVersions -Version $version
        Write-Verbose "Install version '$($newestEdge.Version)' for platform '$($newestEdge.Platform)'"
        $tmpDir = New-TempDirectory
        Start-BitsTransfer -Source $newestEdge.path -Destination $tmpDir
        Get-ChildItem $tmpDir | Copy-Item -Destination $OutputDir -PassThru | Add-FileToProject
        Remove-Item $tmpDir -Force -Recurse
        Save-InstalledDriverInfo -Browser "Edge" -DriverVersionInfo $newestEdge -OutputDir $OutputDir -DriverFileName "Edge.exe"
    }
}

function Get-OperaDriverAvailableFiles {
    param([string]$Platform)
    $relases = Invoke-RestMethod -Method Get -Uri https://api.github.com/repos/operasoftware/operachromiumdriver/releases
    foreach ($release in $relases) {
        $version = $release.name
        foreach ($asset in $release.assets) {
            $nameParts = $asset.name -split "[_\.]"
            if ($nameParts.length -eq 3) {
                $filePlatform = $nameParts[1]
                if (([String]::IsNullOrWhiteSpace($Platform) -ne $true) -and ($Platform -ne $filePlatform)) {
                    continue
                }
                [pscustomobject](@{Version = $version; Platform = $nameParts[1]; Url = $asset.browser_download_url })
            }
        }
    }
}

function Get-OperaDriverVersions {
    param([string]$Platform)
    Get-OperaDriverAvailableFiles -Platform $Platform | Select-Object -Property @{n = "Driver"; e = {"Opera"}}, Version, Platform
}

function Install-OperaDriver {
    [CmdletBinding()]
    param(
        [string]$Platform = "win32",
        [string]$OutputDir
    )
    DynamicParam {
        New-ParameterSet -Params {
            $availableVersions = Get-OperaDriverVersions | Select-Object -ExpandProperty Version
            New-Parameter -Name "Version" -Position 1 -ValidateSet $availableVersions
        }
    }
    process {
        $version = $PsBoundParameters["Version"]
        $Platform = Get-SelectedOrDefault -Selected $Platform -Default "win32"
        $allVersions = Get-OperaDriverAvailableFiles -Platform $Platform
        $selectedDriver = Select-DriverVersion -AvailableDrivers $allVersions -Version $version
        Write-Verbose "Install version '$($selectedDriver.Version)' for platform '$($selectedDriver.Platform)'"
        $driverFileName = "operadriver.exe"
        $tmpDir = New-TempDirectory
        Invoke-RestMethod -Method Get -Uri $selectedDriver.Url -OutFile "$tmpDir\opera.zip"
        Expand-Archive -Path "$tmpDir\opera.zip" -DestinationPath $tmpDir -Force
        Get-ChildItem -Path $tmpDir -Recurse -Filter $driverFileName  | Copy-Item -Destination $OutputDir -PassThru | Add-FileToProject
        Remove-Item -Path $tmpDir -Force -Recurse
        Save-InstalledDriverInfo -Browser "Opera" -DriverVersionInfo $selectedDriver -OutputDir $OutputDir -DriverFileName $driverFileName
    }
}
function Get-FirefoxDriverAvailableFiles {
    param([string]$Platform)
    [System.Net.ServicePointManager]::SecurityProtocol = [System.Net.SecurityProtocolType]::Tls12;
    $relases = Invoke-RestMethod -Method Get -Uri https://api.github.com/repos/mozilla/geckodriver/releases
    foreach ($release in $relases) {
        $version = $release.name
        foreach ($asset in $release.assets) {
            $nameParts = $asset.name -split "[-]"
            if ($nameParts.length -eq 3) {
                $filePlatform = $nameParts[2] -split "\." | Select-Object -First 1
                if (([String]::IsNullOrWhiteSpace($Platform) -ne $true) -and ($Platform -ne $filePlatform)) {
                    continue
                }
                $fileVersion = if ([string]::IsNullOrWhiteSpace($version)) {$nameParts[1]}else {$version}
                $versionNumber = Get-VersionNumber -VersionString $fileVersion
                [pscustomobject](@{Version = $fileVersion; VersionNumber = $versionNumber;  Platform = $filePlatform; Url = $asset.browser_download_url })
            }
        }
    }
}

function Get-FirefoxDriverVersions {
    param([string]$Platform)
    Get-FirefoxDriverAvailableFiles -Platform $Platform | Select-Object -Property @{n = "Driver"; e = {"Firefox"}}, Version, Platform
}

function Install-FirefoxDriver {
    [CmdletBinding()]
    param(
        [string]$Platform = "win32",
        [string]$OutputDir
    )
    DynamicParam {
        New-ParameterSet -Params {
            $availableVersions = Get-FirefoxDriverVersions | Select-Object -ExpandProperty Version
            New-Parameter -Name "Version" -Position 1 -ValidateSet $availableVersions
        }
    }
    process {
        $version = $PsBoundParameters["Version"]
        $Platform = Get-SelectedOrDefault -Selected $Platform -Default "win32"
        $allVersions = Get-FirefoxDriverAvailableFiles -Platform $Platform
        $selectedDriver = Select-DriverVersion -AvailableDrivers $allVersions -Version $version
        Write-Verbose "Install version '$($selectedDriver.Version)' for platform '$($selectedDriver.Platform)'"
        $tmpDir = New-TempDirectory
        $driverArchiveFile = "$tmpDir\gecko.zip"
        $driverFileName = "geckodriver.exe"
        Invoke-RestMethod -Method Get -Uri $selectedDriver.Url -OutFile $driverArchiveFile
        Expand-Archive -Path $driverArchiveFile -DestinationPath $tmpDir -Force
        Get-ChildItem -Path $tmpDir -Recurse -Filter $driverFileName | Copy-Item -Destination $OutputDir -PassThru | Add-FileToProject
        Remove-Item -Path $tmpDir -Force -Recurse
        Save-InstalledDriverInfo -Browser "Firefox" -DriverVersionInfo $selectedDriver -OutputDir $OutputDir -DriverFileName $driverFileName
    }
}

function Install-SeleniumWebDriver {
    [CmdletBinding()]
    param(
        [Parameter(Mandatory = $true)][ValidateSet("Chrome", "PhantomJs", "InternetExplorer", "Edge", "Firefox", "Opera")][string]$Browser,
        [string] $Platform,
        [string] $OutputDir
    )
    DynamicParam {
        New-ParameterSet -Params {
            $availableVersions = Get-SeleniumWebDriverVersions -Browser $Browser -Platform $Platform | Select-Object -ExpandProperty Version
            New-Parameter -Name "Version" -Position 1 -ValidateSet $availableVersions
        }
    }
    process {
        $version = $PsBoundParameters["Version"]
        $driverOutputPath = New-DriversDirectory -Directory $OutputDir
        Write-Verbose "Install driver in  '$driverOutputPath'"
        if ([string]::IsNullOrWhiteSpace($version)) {
            switch ($Browser) {
                "Chrome" {Install-ChromeDriver -Platform $Platform -OutputDir $driverOutputPath; break}
                "PhantomJs" {Install-PhantomJSDriver -Platform $Platform -OutputDir $driverOutputPath; break}
                "InternetExplorer" {Install-IEDriver -Platform $Platform -OutputDir $driverOutputPath; break}
                "Edge" {Install-EdgeDriver -Platform $Platform -OutputDir $driverOutputPath}
                "Firefox" {Install-FirefoxDriver -Platform $Platform -OutputDir $driverOutputPath; break}
                "Opera" {Install-OperaDriver -Platform $Platform -OutputDir $driverOutputPath; break}
                default {"Unsupported browser type. Please select browser from the follwing list: Chrome, PhantomJs, InternetExplorer, Edge, Firefox, Opera"}
            }
        }
        else {
            switch ($Browser) {
                "Chrome" {Install-ChromeDriver -Platform $Platform -Version $version -OutputDir $driverOutputPath; break}
                "PhantomJs" {Install-PhantomJSDriver -Platform $Platform -Version $version -OutputDir $driverOutputPath; break}
                "InternetExplorer" {Install-IEDriver -Platform $Platform -Version $version -OutputDir $driverOutputPath; break}
                "Edge" {Install-EdgeDriver -Platform $Platform  -Version $version -OutputDir $driverOutputPath}
                "Firefox" {Install-FirefoxDriver -Platform $Platform -Version $version -OutputDir $driverOutputPath; break}
                "Opera" {Install-OperaDriver -Platform $Platform -Version $version -OutputDir $driverOutputPath; break}
                default {"Unsupported browser type. Please select browser from the follwing list: Chrome, PhantomJs, InternetExplorer, Edge, Firefox, Opera"}
            }
        }
    }
}

function Get-SeleniumWebDriverVersions {
    [CmdletBinding()]
    param(
        [Parameter(Mandatory = $true, ValueFromPipeline = $true)][ValidateSet("Chrome", "PhantomJs", "InternetExplorer", "Edge", "Firefox", "Opera")][string]$Browser,
        [string]$Platform
    )
    process {
        foreach ($currentBrowser in $Browser) {
            switch ($currentBrowser) {
                "Chrome" {Get-ChromeDriverVersions -Platform $Platform; break}
                "PhantomJs" {Get-PhantomJSDriverVersions -Platform $Platform; break}
                "InternetExplorer" {Get-IEDriverVersions -Platform $Platform; break}
                "Edge" {Get-EdgeDriverVersions; break}
                "Firefox" {Get-FirefoxDriverVersions -Platform $Platform ; break}
                "Opera" {Get-OperaDriverVersions -Platform $Platform; break}
                default { throw "Unsupported browser type. Please select browser from the follwing list: Chrome, PhantomJs, InternetExplorer, Edge, Firefox, Opera"}
            }
        }
    }
}
function Restore-SeleniumWebDriver{
    [CmdletBinding()]
    param($CheckAutoRestore=$false)
    $configFile = Get-InstallerConfigFilePath
    if((Test-Path $configFile) -eq $false)
    {
        return
    }
    $config =  Get-Content -Path $configFile -Raw | ConvertFrom-Json
    if($CheckAutoRestore -and ($config.autoRestore -eq $false))
    {
        return
    }
    $config.drivers | Where-Object { (Test-Path "$($_.outputDir)\$($_.file)") -eq  $false }  `
        | ForEach-Object {
           $driverOutputPath = New-DriversDirectory -Directory $OutputDir
           Install-SeleniumWebDriver -Browser $_.browser -Platform $_.platform -Version $_.version -OutputDir $_.driverOutputPath
         }
}
#Export-ModuleMember -Function Install-SeleniumWebDriver, Get-SeleniumWebDriverVersions, Restore-SeleniumWebDriver
