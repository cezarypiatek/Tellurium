function Install-ChromeDriver{
    $platform = "win32"
    $baseUrl = "http://chromedriver.storage.googleapis.com"
    $p = Invoke-WebRequest "$baseUrl/?prefix="
    $o = [xml]$p.Content 
    $newestFile = $o.ListBucketResult.Contents |% {
    $parts =  $_.Key -split "/"; 
    if(($parts.Length -eq2)  -and($parts[1].EndsWith(".zip")))
    {
        $versionParts =  $parts[0] -split "\."
        [PsCustomObject](@{Version= [int]$versionParts[0]*100 +[int]$versionParts[1]  ; File= "$baseUrl/$($_.Key)"})
    }
    }|? {$_.File -like "*$platform*"} | Sort-Object -Property Version | Select-Object -Last 1

    Start-BitsTransfer -Source $newestFile.File -Destination c:\tmp\chromdriver.zip
}