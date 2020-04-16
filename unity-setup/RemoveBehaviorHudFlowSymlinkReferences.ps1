Import-Module -Name .\HandleReferences -Verbose

#
New-Variable `
    -Name referencedSettingFolder `
    -Value "behavior-hud-flow-referenced-settings" `
    -Option private

New-Variable `
    -Name referenceFolder `
    -Value "references" `
    -Option private

New-Variable `
    -Name projectName `
    -Value "behavior-hud-flow-unity" `
    -Option private

#
Write-Host "Remove References`nProject: $projectName`nSettings: $referencedSettingFolder`nReference: $referenceFolder" `
    -ForegroundColor Blue

#
Symlink-InReferences `
    (Split-Path $script:MyInvocation.MyCommand.Path) `
    $referencedSettingFolder `
    $referenceFolder `
    $projectName `
    ${function:Remove-SymlinkOrNormal}
