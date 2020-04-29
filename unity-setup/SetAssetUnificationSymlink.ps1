Import-Module -Name .\HandleReferences -Verbose

#
New-Variable `
    -Name referencedSettingFolder `
    -Value "asset-unification-referenced-settings" `
    -Option private

New-Variable `
    -Name referenceFolder `
    -Value "references" `
    -Option private

New-Variable `
    -Name projectName `
    -Value "asset-unification-unity" `
    -Option private

#
Write-Host "Set Symlink`nProject: $projectName`nSettings: $referencedSettingFolder`nReference: $referenceFolder" `
    -ForegroundColor Blue

#
Symlink-InReferences `
    (Split-Path $script:MyInvocation.MyCommand.Path) `
    $referencedSettingFolder `
    $referenceFolder `
    $projectName `
    ${function:Set-Symlink}
