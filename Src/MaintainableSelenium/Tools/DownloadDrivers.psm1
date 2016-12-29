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
    Expand-Archive -Path $driverTmpPath -DestinationPath $DestinationPath -Force -Verbose
    Remove-Item -Path $driverTmpPath -Force -Recurse    
}

function Install-ChromeDriver{
    $driversPath = Add-ProjectDirectoryIfNotExist -Project $project -DirPath "Drivers"
    Download-FromGoogleapis -BaseUrl "http://chromedriver.storage.googleapis.com" -DriverName "chromdriver" -DestinationPath $driversPath
}

function Install-IEDriver{
    $driversPath = Add-ProjectDirectoryIfNotExist -Project $project -DirPath "Drivers"
    Download-FromGoogleapis -BaseUrl "http://selenium-release.storage.googleapis.com" -DriverName "IEDriverServer" -DestinationPath  $driversPath
}
