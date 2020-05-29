# Get-ChildItem -recurse -filter .DS_STORE | Remove-Item -WhatIf

function Remove-SpecificFolder {
param(
    [String[]]
    $IncludedNames
    )

    $startPath = (Split-Path $script:MyInvocation.MyCommand.Path)
    $foundPathLocations = Get-Childitem $startPath -Include $IncludedNames -Recurse -Force -ErrorAction SilentlyContinue
    $pathCount = $foundPathLocations.Length
    Write-Output "Remove $pathCount locations from $startPath"
    if ($foundPathLocations.Length -gt 0) {
        foreach ($pathLocation in $foundPathLocations) {
            Write-Host $pathLocation `
                -ForegroundColor DarkRed
        }
        $foundPathLocations | Remove-Item -Force -Recurse
    }
}

$names = @("._*", ".DS_Store", ".fseventsd", ".TemporaryItems", ".Trashes", ".Spotlight-V100")
Remove-SpecificFolder $names
