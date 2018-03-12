$source = "https://netstorage.unity3d.com/unity/a9f86dcd79df/Windows64EditorInstaller/UnitySetup64-2017.3.0f3.exe"
$destination = ".\UnitySetup64.exe"
Invoke-WebRequest $source -OutFile $destination
Start-Process -FilePath ".\UnitySetup64.exe" -Wait -ArgumentList ('/S', '/Q')
Write-Host "$(date) UnitySetup64-2017.3.0f3 Installed"-ForegroundColor green