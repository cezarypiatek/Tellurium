@echo off
set version=1.0.0
if not "%PackageVersion%" == "" (
   set version=%PackageVersion%
)
set config="Release"
nuget pack ..\Src\MvcPages\MvcPages.csproj.nuspec -NoPackageAnalysis -verbosity detailed -o ./ -Version %version% -p Configuration="%config%"
nuget pack ..\Src\VisualAssertions\VisualAssertions.csproj.nuspec -NoPackageAnalysis -verbosity detailed -o ./ -Version %version% -p Configuration="%config%"
nuget pack ..\Src\Tools\SeleniumDriverInstaller.nuspec -NoPackageAnalysis -verbosity detailed -o ./ -Version %version% -p Configuration="%config%"