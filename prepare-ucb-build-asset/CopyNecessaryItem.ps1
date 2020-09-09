# game-unity/prepare-ucb-make-asset
$currentScriptPath = (Split-Path $script:MyInvocation.MyCommand.Path)
# game-unity
$parentOfCurrentPath = (Get-Item $currentScriptPath).Parent.FullName

function Remove-UnityProjectUncessaryPart {
param(
    [String[]]
    $IncludedNames
    )

    # $unityProjectPath = Join-Path -Path $currentScriptPath -ChildPath "complete-unity"
    $assetManipulationUnityFolder = Join-Path -Path $parentOfCurrentPath -ChildPath "asset-unification-unity"
    $foundPathLocations = Get-Childitem $assetManipulationUnityFolder -Include $IncludedNames -Recurse -Force -ErrorAction SilentlyContinue
    $pathCount = $foundPathLocations.Length
    Write-Output "Remove $pathCount localtion from $assetManipulationUnityFolder"
    if ($foundPathLocations.Length -gt 0) {
        foreach ($pathLocation in $foundPathLocations) {
            Remove-Item $pathLocation -Recurse -Force
            Write-Host $pathLocation `
                -ForegroundColor DarkRed
        }
    }
}

$names = @(".idea", ".gitignore")
Remove-UnityProjectUncessaryPart $names

function Copy-ItemIn {
param()
    $supplyToProjectFolder = Join-Path -Path $currentScriptPath -ChildPath "supply-to-project"

    # Get references to be symlinked
    $settingsFolder = Join-Path -Path $supplyToProjectFolder -ChildPath "settings"
    $gameSpecificJsonFile = Join-Path -Path $settingsFolder -ChildPath "game-specific.json"

    $projectFolder = Join-Path -Path $supplyToProjectFolder -ChildPath "project"
    $packagesFolder = Join-Path -Path $projectFolder -ChildPath "Packages"
    $manifestJsonFile = Join-Path -Path $packagesFolder -ChildPath "manifest.json"

    $scriptsFolder = Join-Path -Path $projectFolder -ChildPath "scripts"

    # Set references to be symlinked
    $assetManipulationUnityFolder = Join-Path -Path $parentOfCurrentPath -ChildPath "asset-unification-unity"
    $amuScriptsFolder = Join-Path -Path $assetManipulationUnityFolder -ChildPath "scripts"
    $amuPackagesFolder = Join-Path -Path $assetManipulationUnityFolder -ChildPath "Packages"
    $amupManifestJsonFile = Join-Path -Path $amuPackagesFolder -ChildPath "manifest.json"

    #
    if (Test-Path "$amuScriptsFolder") {
        (Get-Item "$amuScriptsFolder").Delete()
    }

    Copy-Item $scriptsFolder -Destination $assetManipulationUnityFolder -Recurse

    if (Test-Path "$amupManifestJsonFile") {
        (Get-Item "$amupManifestJsonFile").Delete()
    }

    Copy-Item $manifestJsonFile -Destination $amuPackagesFolder


    #
    $projectGitattributesFile = Join-Path -Path $projectFolder -ChildPath "gitattributes"
    $projectGitignoreFile = Join-Path -Path $projectFolder -ChildPath "gitignore"

    $amupAssetGitattributesFile = Join-Path -Path $assetManipulationUnityFolder -ChildPath ".gitattributes"
    $amupAssetsGitignore = Join-Path -Path $assetManipulationUnityFolder -ChildPath ".gitignore"

    if (Test-Path "$amupAssetGitattributesFile") {
        (Get-Item "$amupAssetGitattributesFile").Delete()
    }

    if (Test-Path "$amupAssetsGitignore") {
        # (Get-Item "$amupAssetsGitignore").Delete()
        Remove-Item -Path "$amupAssetsGitignore" -Force
    }

    #
    Move-Item -Path $projectGitattributesFile -Destination $amupAssetGitattributesFile
    Move-Item -Path $projectGitignoreFile -Destination $amupAssetsGitignore

    #
    $unitySetupFolder = Join-Path -Path $parentOfCurrentPath -ChildPath "unity-setup"
    $amrsFolder = Join-Path -Path $unitySetupFolder -ChildPath "asset-unification-referenced-settings"
    $amrsGameSpecificJsonFile = Join-Path -Path $amrsFolder -ChildPath "game-specific.json"

    if (Test-Path "$amrsGameSpecificJsonFile") {
        (Get-Item "$amrsGameSpecificJsonFile").Delete()
    }

    Copy-Item $gameSpecificJsonFile -Destination $amrsFolder
}

Copy-ItemIn

