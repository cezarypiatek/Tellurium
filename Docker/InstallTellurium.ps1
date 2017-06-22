param([switch]$InstallService=$false)
New-Item c:\Downloads -ItemType Directory | Out-Null
$data = Invoke-RestMethod -Method Get -Uri https://api.github.com/repos/cezarypiatek/Tellurium/releases/latest
$downloadSource = $data.assets | Where-Object {$_.name -eq "TelluriumDashboard.zip"} | Select-Object -First 1
Invoke-RestMethod -Method Get -Uri $downloadSource.browser_download_url -OutFile "c:\Downloads\TelluriumDashboard.zip"
Expand-Archive -Path "c:\Downloads\TelluriumDashboard.zip" -DestinationPath "c:\TelluriumDashboard\" 
if($InstallService)
{
	. c:\TelluriumDashboard\Dashboard\TelluriumDashboard.exe install -servicename "TelluriumDashboard"
	. c:\TelluriumDashboard\Dashboard\TelluriumDashboard.exe start
}
