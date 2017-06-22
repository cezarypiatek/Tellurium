function Set-XmlElements
{
    param([string]$Path, [scriptblock]$UpdateFunc)
    [xml]$xml = Get-Content $path 
    & $UpdateFunc $xml
    $xml.Save($path)
}

if([string]::IsNullOrWhiteSpace($env:VISUALASSERTION_CONNECTION_STRING))
{
	throw "VISUALASSERTION_CONNECTION_STRING variable is not defined"
}

Set-XmlElements -Path 'c:\TelluriumDashboard\Dashboard\TelluriumDashboard.exe.config' -UpdateFunc {
    param($xml)	
    $xml.configuration.'hibernate-configuration'.'session-factory'.FirstChild.InnerText = $env:VISUALASSERTION_CONNECTION_STRING    
}
. c:\TelluriumDashboard\Dashboard\TelluriumDashboard.exe run