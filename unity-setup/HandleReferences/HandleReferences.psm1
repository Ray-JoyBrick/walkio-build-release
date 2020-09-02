New-Variable `
    -Name showMetaSymlinkResult `
    -Value $FALSE `
    -Option private

function Set-Symlink {
param(
    [String]
    $TargetLocation,
    [String]
    $TargetMetaLocation,
    [String]
    $SourceLocation,
    [String]
    $SourceMetaLocation
    )

    #
    New-Variable `
        -Name targetLocationFile `
        -Value (Get-Item $TargetLocation -Force -ea SilentlyContinue) `
        -Option private

    #
    New-Variable `
        -Name showText `
        -Value "" `
        -Option private

    # Remove symlinked file/folder
    if ([bool]($targetLocationFile.Attributes -band [IO.FileAttributes]::ReparsePoint)) {
        (Get-Item $targetLocationFile).Delete()
    } else {
        if ($targetLocationFile) {
            $showText = "Can not remove symlink, not symlink at: $targetLocationFile"
            Write-Host $showText `
                -ForegroundColor DarkCyan
        }
    }

    if (Test-Path "$TargetMetaLocation") {
        Remove-Item $TargetMetaLocation
    } else {
        $showText = "Can not remove meta, not meta found at: $TargetMetaLocation"
        if ($showMetaSymlinkResult) {
            Write-Host $showText `
                -ForegroundColor DarkGray
        }
    }

    # Symlink file/folder if there is any
    if (Test-Path "$SourceLocation") {
        New-Item -ItemType SymbolicLink -Path $TargetLocation -Value $SourceLocation
    } else {
        $showText = "Can not make symlink for file or folder`nSource: $SourceLocation`nTarget: $TargetLocation"
        Write-Host $showText `
            -ForegroundColor DarkRed `
            -BackgroundColor White
    }
    # Symlink meta file if there is any
    if (Test-Path "$SourceMetaLocation") {
        New-Item -ItemType SymbolicLink -Path $TargetMetaLocation -Value $SourceMetaLocation
    } else {
        $showText = "Can not make symlink for meta`nSource: $SourceLocation`nTarget: $TargetLocation"
        if ($showMetaSymlinkResult) {
            Write-Host $showText `
                -ForegroundColor DarkGray
        }
    }
}

function Copy-Asset {
param(
    [String]
    $TargetLocation,
    [String]
    $TargetMetaLocation,
    [String]
    $SourceLocation,
    [String]
    $SourceMetaLocation
    )

    #
    New-Variable `
        -Name targetLocationFile `
        -Value (Get-Item $TargetLocation -Force -ea SilentlyContinue) `
        -Option private

    #
    New-Variable `
        -Name showText `
        -Value "" `
        -Option private

    # Remove symlinked file/folder
    if ([bool]($targetLocationFile.Attributes -band [IO.FileAttributes]::ReparsePoint)) {
        (Get-Item $targetLocationFile).Delete()
    } else {
        if ($targetLocationFile) {
            $showText = "Can not remove symlink, not symlink at path: $TargetLocation"
            Write-Host $showText `
                -ForegroundColor DarkCyan
            if ($targetLocationFile) {
                if (Test-Path $targetLocationFile) {
                    Remove-Item $targetLocationFile -Recurse -Force
                    $showText = "Normal folder/file, can remove: $targetLocationFile"
                    Write-Host $showText `
                        -ForegroundColor Cyan
                }
            }
        }
    }

    if (Test-Path "$TargetMetaLocation") {
        Remove-Item $TargetMetaLocation
    } else {
        $showText = "Can not remove meta, not meta found at: $TargetMetaLocation"
        if ($showMetaSymlinkResult) {
            Write-Host $showText `
                -ForegroundColor DarkGray
        }
    }

    # Symlink file/folder if there is any
    if (Test-Path "$SourceLocation") {
        Copy-Item $SourceLocation -Destination $TargetLocation -Recurse
        $showText = "Copy from`n$SourceLocation to`n$TargetLocation"
        Write-Host $showText `
            -ForegroundColor DarkBlue
    } else {
        $showText = "Can not make symlink for file or folder`nSource: $SourceLocation`nTarget: $TargetLocation"
        Write-Host $showText `
            -ForegroundColor DarkRed `
            -BackgroundColor White
    }
    # Symlink meta file if there is any
    if (Test-Path "$SourceMetaLocation") {
        Copy-Item $SourceLocaSourceMetaLocationtion -Destination $TargetMetaLocation
    } else {
        $showText = "Can not make symlink for meta`nSource: $SourceLocation`nTarget: $TargetLocation"
        if ($showMetaSymlinkResult) {
            Write-Host $showText `
                -ForegroundColor DarkGray
        }
    }
}

function Remove-SymlinkOrNormal {
param(
    [String]
    $TargetLocation,
    [String]
    $TargetMetaLocation,
    [String]
    $SourceLocation,
    [String]
    $SourceMetaLocation
    )

    #
    New-Variable `
        -Name targetLocationFile `
        -Value (Get-Item $TargetLocation -Force -ea SilentlyContinue) `
        -Option private

    #
    New-Variable `
        -Name showText `
        -Value "" `
        -Option private

    if ([bool]($targetLocationFile.Attributes -band [IO.FileAttributes]::ReparsePoint)) {
        (Get-Item $targetLocationFile).Delete()
        $showText = "Can remove symlink: $targetLocationFile"
        Write-Host $showText `
            -ForegroundColor DarkCyan
    } else {
        $showText = "Can not remove symlink, not symlink at path: $TargetLocation"
        Write-Host $showText `
            -ForegroundColor DarkGray
        if ($targetLocationFile) {
            if (Test-Path $targetLocationFile) {
                Remove-Item $targetLocationFile -Recurse -Force
                $showText = "Normal folder/file, can remove: $targetLocationFile"
                Write-Host $showText `
                    -ForegroundColor Cyan
            }
        }
    }

    if (Test-Path "$TargetMetaLocation") {
        Remove-Item $TargetMetaLocation
        $showText = "Remove meta at: $targetLocationFile"
        if ($showMetaSymlinkResult) {
            Write-Host $showText `
            -ForegroundColor DarkGray
        }
    } else {
        $showText = "Can not remove meta, not meta found at: $TargetMetaLocation"
        if ($showMetaSymlinkResult) {
            Write-Host $showText `
                -ForegroundColor DarkGray
        }
    }
}

function Make-Setup {
param(
    [String]
    $ParentOfScriptDir,
    [String]
    $TargetProjectName,
    $SymlinkItem,
    [String]
    $SourceBase,
    $WorkFunction
    )

    #
    New-Variable `
        -Name name `
        -Value $SymlinkItem.name `
        -Option private

    New-Variable `
        -Name toLocation `
        -Value $SymlinkItem.to `
        -Option private

    #
    New-Variable `
        -Name sourceLocation `
        -Value (Join-Path -Path $SourceBase -ChildPath $name) `
        -Option private

    New-Variable `
        -Name sourceMetaLocation `
        -Value "$name.meta" `
        -Option private

    #
    New-Variable `
        -Name targetProjectPath `
        -Value (Join-Path -Path $ParentOfScriptDir -ChildPath $TargetProjectName) `
        -Option private

    #
    New-Variable `
        -Name targetLocation `
        -Value (Join-Path -Path $targetProjectPath -ChildPath $toLocation) `
        -Option private

    New-Variable `
        -Name targetMetaLocation `
        -Value (Join-Path -Path $targetProjectPath -ChildPath "$toLocation.meta") `
        -Option private

    Invoke-Command $WorkFunction -ArgumentList $targetLocation, $targetMetaLocation, $sourceLocation, $sourceMetaLocation

    # Set-Symlink $targetLocation $targetMetaLocation $sourceLocation $sourceMetaLocation
    # Remove-SymlinkOrNormal $targetLocation $targetMetaLocation $sourceLocation $sourceMetaLocation
}

function Process-SingleFile {
param(
    [String]
    $ParentOfScriptDir,
    [String]
    $InReferencesBase,
    [String]
    $TargetProjectName,
    $File,
    $WorkFunction
    )

    #
    New-Variable `
        -Name setting `
        -Value ((Get-Content $File) | ConvertFrom-Json) `
        -Option private

    # Write-Output $setting

    New-Variable `
        -Name sourceBase `
        -Value (Join-Path -Path $InReferencesBase -ChildPath $setting.name) `
        -Option private

    #
    if ($setting.use) {
        foreach ($symlinkItem in $setting.toSymlink) {
            Make-Setup $ParentOfScriptDir $TargetProjectName $symlinkItem $sourceBase $WorkFunction
        }
    }
}

function Symlink-InReferences {
param(
    [String]
    $ScriptDir,
    [String]
    $JsonFolderName,
    [String]
    $ReferenceFolderName,
    [String]
    $TargetProjectName,
    $WorkFunction
    )

    #
    New-Variable `
        -Name jsonFolderPath `
        -Value (Join-Path -Path $ScriptDir -ChildPath $JsonFolderName) `
        -Option private

    New-Variable `
        -Name parentOfScriptDir `
        -Value ((Get-Item $ScriptDir ).Parent.FullName) `
        -Option private

    New-Variable `
        -Name inReferencesBase `
        -Value (Join-Path -Path $parentOfScriptDir -ChildPath $ReferenceFolderName) `
        -Option private

    New-Variable `
        -Name files `
        -Value @(Get-ChildItem "$jsonFolderPath\*.json") `
        -Option private

    #
    foreach ($file in $files) {
        Process-SingleFile $parentOfScriptDir $inReferencesBase $TargetProjectName $file $WorkFunction
    }
}

function Get-AssetToDesc {
param(
    [String]
    $TargetLocation,
    [String]
    $TargetMetaLocation,
    [String]
    $SourceLocation,
    [String]
    $SourceMetaLocation,
    [ref]
    $outputDesc
    )
    
    Write-Host "Get-AssetToDesc" `
        -ForegroundColor DarkBlue

    # #
    # New-Variable `
    #     -Name targetLocationFile `
    #     -Value (Get-Item $TargetLocation -Force -ea SilentlyContinue) `
    #     -Option private

    #
    New-Variable `
        -Name showText `
        -Value "" `
        -Option private

    # Remove symlinked file/folder
    # if ([bool]($targetLocationFile.Attributes -band [IO.FileAttributes]::ReparsePoint)) {
    #     (Get-Item $targetLocationFile).Delete()
    # } else {
    #     if ($targetLocationFile) {
    #         $showText = "Can not remove symlink, not symlink at path: $TargetLocation"
    #         Write-Host $showText `
    #             -ForegroundColor DarkCyan
    #         if ($targetLocationFile) {
    #             if (Test-Path $targetLocationFile) {
    #                 Remove-Item $targetLocationFile -Recurse -Force
    #                 $showText = "Normal folder/file, can remove: $targetLocationFile"
    #                 Write-Host $showText `
    #                     -ForegroundColor Cyan
    #             }
    #         }
    #     }
    # }

    # if (Test-Path "$TargetMetaLocation") {
    #     Remove-Item $TargetMetaLocation
    # } else {
    #     $showText = "Can not remove meta, not meta found at: $TargetMetaLocation"
    #     if ($showMetaSymlinkResult) {
    #         Write-Host $showText `
    #             -ForegroundColor DarkGray
    #     }
    # }

    # Symlink file/folder if there is any
    # if (Test-Path "$SourceLocation") {
        $outputDesc.Value += "mv '$SourceLocation' '$TargetLocation'"
        $outputDesc.Value += "`n"

        # Copy-Item $SourceLocation -Destination $TargetLocation -Recurse
        # $showText = "Copy from`n$SourceLocation to`n$TargetLocation"
        # Write-Host $showText `
        #     -ForegroundColor DarkBlue
    # } else {
        # $showText = "Can not make symlink for file or folder`nSource: $SourceLocation`nTarget: $TargetLocation"
        # Write-Host $showText `
        #     -ForegroundColor DarkRed `
        #     -BackgroundColor White
    # }
    # Symlink meta file if there is any
    # if (Test-Path "$SourceMetaLocation") {
        $outputDesc.Value += "mv '$SourceMetaLocation' '$TargetMetaLocation'"
        $outputDesc.Value += "`n"

        # Copy-Item $SourceLocaSourceMetaLocationtion -Destination $TargetMetaLocation
    # } else {
        # $showText = "Can not make symlink for meta`nSource: $SourceLocation`nTarget: $TargetLocation"
        # if ($showMetaSymlinkResult) {
        #     Write-Host $showText `
        #         -ForegroundColor DarkGray
        # }
    # }
}

function Make-SetupB {
param(
    [String]
    $ParentOfScriptDir,
    [String]
    $TargetProjectName,
    [String]
    $settingName,
    $SymlinkItem,
    [String]
    $SourceBase,
    [ref]
    $outputDesc,
    $WorkFunction
    )

    #
    New-Variable `
        -Name name `
        -Value $SymlinkItem.name `
        -Option private

    New-Variable `
        -Name toLocation `
        -Value $SymlinkItem.to `
        -Option private

    #
    New-Variable `
        -Name sourceLocation `
        -Value (Join-Path -Path "./references/$settingName" -ChildPath $name) `
        -Option private

    New-Variable `
        -Name sourceMetaLocation `
        -Value (Join-Path -Path "./references/$settingName" -ChildPath "$name.meta") `
        -Option private

    #
    # New-Variable `
    #     -Name targetProjectPath `
    #     -Value (Join-Path -Path $ParentOfScriptDir -ChildPath $TargetProjectName) `
    #     -Option private

    #
    New-Variable `
        -Name targetLocation `
        -Value (Join-Path -Path "." -ChildPath $toLocation) `
        -Option private

    New-Variable `
        -Name targetMetaLocation `
        -Value (Join-Path -Path "." -ChildPath "$toLocation.meta") `
        -Option private

    Invoke-Command $WorkFunction -ArgumentList $targetLocation, $targetMetaLocation, $sourceLocation, $sourceMetaLocation, $outputDesc

    # Set-Symlink $targetLocation $targetMetaLocation $sourceLocation $sourceMetaLocation
    # Remove-SymlinkOrNormal $targetLocation $targetMetaLocation $sourceLocation $sourceMetaLocation
}
    
function Process-SingleFileB {
param(
    [String]
    $ParentOfScriptDir,
    [String]
    $InReferencesBase,
    [String]
    $TargetProjectName,
    $File,
    [ref]
    $outputDesc,
    $WorkFunction
    )

    #
    New-Variable `
        -Name setting `
        -Value ((Get-Content $File) | ConvertFrom-Json) `
        -Option private

    # Write-Output $setting

    New-Variable `
        -Name sourceBase `
        -Value (Join-Path -Path $InReferencesBase -ChildPath $setting.name) `
        -Option private

    #
    if ($setting.use) {
        foreach ($symlinkItem in $setting.toSymlink) {
            Make-SetupB $ParentOfScriptDir $TargetProjectName $setting.name $symlinkItem $sourceBase $outputDesc $WorkFunction
        }
    }
}

function Make-MoveDesc {
param(
    [String]
    $ScriptDir,
    [String]
    $JsonFolderName,
    [String]
    $ReferenceFolderName,
    [String]
    $TargetProjectName,
    [ref]
    $outputDesc,
    $WorkFunction
    )

    #
    New-Variable `
        -Name jsonFolderPath `
        -Value (Join-Path -Path $ScriptDir -ChildPath $JsonFolderName) `
        -Option private

    New-Variable `
        -Name parentOfScriptDir `
        -Value ((Get-Item $ScriptDir ).Parent.FullName) `
        -Option private

    New-Variable `
        -Name inReferencesBase `
        -Value (Join-Path -Path $parentOfScriptDir -ChildPath $ReferenceFolderName) `
        -Option private

    New-Variable `
        -Name files `
        -Value @(Get-ChildItem "$jsonFolderPath\*.json") `
        -Option private

    #
    foreach ($file in $files) {
        Process-SingleFileB $parentOfScriptDir $inReferencesBase $TargetProjectName $file $outputDesc $WorkFunction
    }
}
    