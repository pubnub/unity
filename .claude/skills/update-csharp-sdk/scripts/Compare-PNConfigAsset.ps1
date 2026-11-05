<#
.SYNOPSIS
    Compares the C# SDK's PNConfiguration with the Unity PNConfigAsset ScriptableObject.

.DESCRIPTION
    PNConfigAsset mirrors a subset of PNConfiguration so it can be edited in the Unity inspector.
    Three places must agree: the asset's public fields, its implicit conversion to PNConfiguration,
    and the propNames list in PNConfigAssetEditor that decides what the inspector draws.

    Reads PNConfiguration's public settable properties (and their defaults) from a PubnubApiUnity.dll
    by reflection and reports:
      FAIL  asset field named like a PNConfiguration property but not mapped in the conversion
      FAIL  conversion maps a property that the DLL no longer has
      FAIL  serialized asset field missing from the editor's propNames (or a propName with no field)
      WARN  asset default differs from the C# default (may be intentional, e.g. "demo" keys)
      INFO  PNConfiguration properties the asset doesn't expose (decide whether it should)

    Exits with 1 when any FAIL is reported.

.PARAMETER DllPath
    DLL to read PNConfiguration from. Defaults to the imported Runtime/Plugins/PubnubApiUnity.dll; pass a
    freshly built C# SDK DLL to see what a new import requires before copying it.

.EXAMPLE
    powershell -ExecutionPolicy Bypass -File .claude/skills/update-csharp-sdk/scripts/Compare-PNConfigAsset.ps1
#>
[CmdletBinding()]
param(
    [string]$DllPath,
    [string]$AssetFile,
    [string]$EditorFile
)

$ErrorActionPreference = 'Stop'
$repoRoot = (git rev-parse --show-toplevel).Trim()
Set-Location $repoRoot

$pubnubRoot = 'PubNubUnity/Assets/PubNub'
if (-not $DllPath) { $DllPath = "$pubnubRoot/Runtime/Plugins/PubnubApiUnity.dll" }
if (-not $AssetFile) { $AssetFile = "$pubnubRoot/Runtime/Util/PNConfigAsset.cs" }
if (-not $EditorFile) { $EditorFile = "$pubnubRoot/Editor/PNConfigAssetEditor.cs" }

# Fields that exist only on the Unity side, consumed by PubnubUnityUtils.NewUnityPubnub().
$unityOnlyFields = @('EnableWebGLBuildMode', 'LogToUnityConsole', 'UserId')

# --- C# side -------------------------------------------------------------------------------------
$assembly = [System.Reflection.Assembly]::LoadFrom((Resolve-Path $DllPath).Path)
$configType = $assembly.GetType('PubnubApi.PNConfiguration')
$userId = [Activator]::CreateInstance($assembly.GetType('PubnubApi.UserId'), @('compare-script'))
$defaultConfig = [Activator]::CreateInstance($configType, @($userId))

$csharp = @{}
foreach ($property in $configType.GetProperties([System.Reflection.BindingFlags]'Public,Instance')) {
    if (-not $property.CanWrite -or -not $property.GetSetMethod()) { continue }
    $value = $null
    try { $value = $property.GetValue($defaultConfig) } catch { }
    $csharp[$property.Name] = [pscustomobject]@{
        Type     = $property.PropertyType
        Default  = $value
        Obsolete = $property.GetCustomAttributes([System.ObsoleteAttribute], $false).Count -gt 0
    }
}

# --- Unity side ----------------------------------------------------------------------------------
$assetSource = Get-Content -Raw $AssetFile
$fields = @{}
foreach ($match in [regex]::Matches($assetSource, '(?m)^\s*(\[[^\]]+\]\s*)*public\s+([\w\.<>]+)\s+(\w+)\s*(=\s*([^;]+))?;')) {
    $fields[$match.Groups[3].Value] = [pscustomobject]@{
        Type        = $match.Groups[2].Value
        Initializer = if ($match.Groups[5].Success) { $match.Groups[5].Value.Trim() } else { $null }
        Serialized  = $match.Groups[1].Value -notmatch 'NonSerialized'
    }
}
$mapped = @{}
foreach ($match in [regex]::Matches($assetSource, 'config\.(\w+)\s*=\s*asset\.(\w+)')) {
    $mapped[$match.Groups[1].Value] = $match.Groups[2].Value
}
$editorSource = Get-Content -Raw $EditorFile
$propNamesBlock = [regex]::Match($editorSource, 'propNames\s*=\s*new\[\]\s*\{([^}]*)\}').Groups[1].Value
$propNames = @([regex]::Matches($propNamesBlock, '"(\w+)"') | ForEach-Object { $_.Groups[1].Value })

function Format-Default($type, $value) {
    if ($null -eq $value) { return '' }
    if ($type.IsEnum) { return [Enum]::GetName($type, $value) }
    if ($value -is [bool]) { return $value.ToString().ToLowerInvariant() }
    return [string]$value
}

function Format-Initializer($field, $csharpType) {
    if ($null -ne $field.Initializer) {
        $text = $field.Initializer.Trim('"')
        return ($text -replace '^[\w\.]+\.(?=\w+$)', '')
    }
    # No initializer: Unity serializes the type's default value.
    if ($csharpType -and $csharpType.IsEnum) { return [Enum]::GetName($csharpType, [Enum]::ToObject($csharpType, 0)) }
    switch ($field.Type) { 'bool' { return 'false' } 'int' { return '0' } default { return '' } }
}

# --- Report --------------------------------------------------------------------------------------
$failures = 0
Write-Host "PNConfiguration ($DllPath): $($csharp.Count) settable properties; PNConfigAsset: $($fields.Count) fields, $($mapped.Count) mapped, $($propNames.Count) shown in the inspector"

foreach ($name in ($fields.Keys | Sort-Object)) {
    if ($unityOnlyFields -contains $name) { continue }
    if ($csharp.ContainsKey($name) -and -not $mapped.ContainsKey($name)) {
        Write-Host "  FAIL  field '$name' is not mapped in the PNConfiguration conversion" -ForegroundColor Red; $failures++
    }
}
foreach ($name in ($mapped.Keys | Sort-Object)) {
    if (-not $csharp.ContainsKey($name)) {
        Write-Host "  FAIL  conversion sets '$name', which PNConfiguration no longer has" -ForegroundColor Red; $failures++
    }
}
foreach ($name in ($fields.Keys | Sort-Object)) {
    if ($fields[$name].Serialized -and $propNames -notcontains $name) {
        Write-Host "  FAIL  serialized field '$name' is missing from PNConfigAssetEditor.propNames" -ForegroundColor Red; $failures++
    }
}
foreach ($name in $propNames) {
    if (-not $fields.ContainsKey($name)) {
        Write-Host "  FAIL  PNConfigAssetEditor.propNames lists '$name', which PNConfigAsset doesn't have" -ForegroundColor Red; $failures++
    }
}
foreach ($name in ($mapped.Keys | Sort-Object)) {
    if (-not $csharp.ContainsKey($name) -or -not $fields.ContainsKey($mapped[$name])) { continue }
    $info = $csharp[$name]
    $unityDefault = Format-Initializer $fields[$mapped[$name]] $info.Type
    $csharpDefault = Format-Default $info.Type $info.Default
    if ($unityDefault -ne $csharpDefault) {
        Write-Host "  WARN  default of '$name' differs: asset '$unityDefault', C# '$csharpDefault'" -ForegroundColor Yellow
    }
    if ($info.Obsolete) {
        Write-Host "  WARN  '$name' is [Obsolete] in the C# SDK" -ForegroundColor Yellow
    }
}
$notExposed = @($csharp.Keys | Where-Object { -not $mapped.ContainsKey($_) -and $_ -ne 'UserId' } | Sort-Object)
if ($notExposed.Count -gt 0) {
    Write-Host "  INFO  not exposed by PNConfigAsset (expose user-facing, Unity-serializable settings):"
    foreach ($name in $notExposed) {
        $suffix = if ($csharp[$name].Obsolete) { ' [Obsolete]' } else { '' }
        Write-Host ("          {0} ({1}){2}" -f $name, $csharp[$name].Type.Name, $suffix)
    }
}

if ($failures -gt 0) { exit 1 }
Write-Host "OK: PNConfigAsset fields, conversion and inspector list are consistent"
exit 0
