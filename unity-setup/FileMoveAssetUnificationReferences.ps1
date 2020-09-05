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
Write-Host "Make Move Instruction References`nProject: $projectName`nSettings: $referencedSettingFolder`nReference: $referenceFolder" `
    -ForegroundColor Blue

$outputDesc = ""

#
Make-MoveDesc `
    (Split-Path $script:MyInvocation.MyCommand.Path) `
    $referencedSettingFolder `
    $referenceFolder `
    $projectName `
    ([ref] $outputDesc) `
    ${function:Get-AssetToDesc}

Set-Content -Path '../move-files.sh' -Value $outputDesc
