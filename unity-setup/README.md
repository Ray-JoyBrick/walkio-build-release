# Project Overview

Powershell script for assist the development of the project.

Pass function as parameter is a bit complex, [this article][1000001] has organized very well on this topic.

To show colorful text, use ```Write-Host``` with arguments, the document can be found [here][1000002]. Other developer also make utility function to get color, detail can be found in this [article][1000003].

To split a long line into multiple lines, can use back tick characther. Other method can be found in this [discussion][1000004].

```ps1
Write-Host $showText `
    -ForegroundColor DarkRed`
    -BackgroundColor White
```

Powershell has scope concept, although not necessary, it is used all over the place in this modification. It looks like

```ps1
New-Variable `
    -Name showText `
    -Value "" `
    -Option private
```

Check out the official [document][1000005] for more detail.

A better approach to get where the running script location is

```ps1
$ScriptDir = Split-Path $script:MyInvocation.MyCommand.Path
```

More to read from this [article][1000006] and this [document][1000007].

Each parameter into function can have attributes, it is like this

```ps1
function Do-Something {
param(
    [String]
    $Location
    )
}
```

Read this [article][1000008] to know more.

The use of ```echo``` in powershell is no longer suggested, use ```Write-Output``` or ```Write-Host```, see this [discussion][1000009] for more detail. Document for ```Write-Output``` can be found [here][1000010].

Installing latest powershell detail can be found in this [document][1000011]. On mac platform, using brew is the best and easiest solution.

```sh
brew tap homebrew/cask-versions
brew cask install powershell-preview
```

Currently, powershell version is 7 beta. To use it, type

```sh
pwsh-preview
```

to enter REPL or just given file after the command to execute it right away.

The modification to powershell script adopt the use of Module, but it is not system kind of module, so the purpose is just to organize the code. However, since it is a Module, the form should be looked like Module instead of just a bunch of files with ```.ps1``` as file extension. To make it more like the real Module, both ```.psm1``` and ```.psd1``` files are created. Check this [article][1000012] for more detail how to make Module step by step.

[1000001]: https://n3wjack.net/2018/03/14/passing-a-function-as-a-parameter-in-powershell/
[1000002]: https://docs.microsoft.com/en-us/powershell/module/microsoft.powershell.utility/write-host?view=powershell-6
[1000003]: https://www.networkadm.in/easily-display-powershell-console-colors/
[1000004]: https://stackoverflow.com/questions/2608144/how-to-split-long-commands-over-multiple-lines-in-powershell
[1000005]: https://docs.microsoft.com/en-us/powershell/module/microsoft.powershell.core/about/about_scopes?view=powershell-6
[1000006]: http://ravinderjaiswal.com/scripting/get-the-current-script-directory-in-powershell-vbscript-and-batch/
[1000007]: https://docs.microsoft.com/en-us/powershell/scripting/samples/managing-current-location?view=powershell-6
[1000008]: https://4sysops.com/archives/the-powershell-function-parameters-data-types-return-values/
[1000009]: https://stackoverflow.com/questions/707646/echo-equivalent-in-powershell-for-script-testing
[1000010]: https://docs.microsoft.com/en-us/powershell/module/microsoft.powershell.utility/write-output?view=powershell-6
[1000011]: https://docs.microsoft.com/en-us/powershell/scripting/install/installing-powershell-core-on-macos?view=powershell-6
[1000012]: https://powershellexplained.com/2017-05-27-Powershell-module-building-basics/
