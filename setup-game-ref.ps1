Param(
  [switch]$Verbose = $false
)

# Explicitly turn on $VerbosePreference
$previousVerbosePreference = $VerbosePreference
if ($Verbose)
{
  $VerbosePreference = "continue"
}

Function Set-ReferenceSymlink
{
  Param(
    [string]$GameFolderName = $false,

    [string]$SourceFolderName = $false
  )

  $currentPath = (Get-Item -Path ".\" -Verbose).FullName
  $referencesPath = Join-Path -Path $currentPath -ChildPath "references"

  $sourePath = Join-Path -Path $currentPath -ChildPath $SourceFolderName

  if (Test-Path $referencesPath)
  {
    Write-Verbose "Folder exists at $referencesPath"
  }
  else
  {
    # Write-Verbose "Make sure folder exists"
    Write-Error "Make sure folder exist at $referencesPath"
    return
  }

  if (Test-Path $SourceFolderName)
  {
    Write-Verbose "Folder exists at $SourceFolderName"
  }
  else
  {
    # Write-Verbose "Make sure folder exists"
    Write-Error "Make sure folder exist at $SourceFolderName"
    return
  }

  $files = @(Get-ChildItem "$sourceSettingsPath\*.json")

  $targetFolderPath = Join-Path -Path $referencesPath -ChildPath $GameFolderName

  $targetFolderPathFile = Get-Item $targetFolderPath -Force -ea SilentlyContinue

  if ([bool]($targetFolderPathFile.Attributes -band [IO.FileAttributes]::ReparsePoint))
  {
    (Get-Item $targetFolderPath).Delete()
    Write-Verbose "$targetFolderPath is Symlink and is removed"
  }
  else
  {
    Write-Verbose "$targetFolderPath is not a symlink, can not remove it"
    Write-Verbose "However, based on the rule, this should be a symlink, check it to maker sure"
  }

  if (Test-Path $sourePath)
  {
    New-Item -ItemType SymbolicLink -Path $targetFolderPath -Value $sourePath
    Write-Verbose "Symlink created from $sourePath to $targetFolderPath"
  }
  else
  {
    Write-Warning "Configure setting indicates $sourePath should exists but its not"
  }
}

Set-ReferenceSymlink -GameFolderName "game-specific" -SourceFolderName "game-specific"

$VerbosePreference = $previousVerbosePreference
