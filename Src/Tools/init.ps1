param($installPath, $toolsPath, $package)
Import-Module (Join-Path $toolsPath SeleniumDriverInstaller.psm1)
Restore-SeleniumWebDriver -CheckAutoRestore $true