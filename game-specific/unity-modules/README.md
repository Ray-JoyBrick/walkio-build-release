# Overview

## Package

### Referenced package

Module asmdef裡的寫法需要註釋一下，拿Level當例子

```json
{
    "defineConstraints": [
        "WALKIO_LEVEL"
    ],
    "versionDefines": [
        {
            "name": "com.walkio.game.level.common",
            "expression": "0.2.0",
            "define": "WALKIO_LEVEL"
        }
    ]
}
```

在這裡**defineConstraints**代表若是有這個PreProcessor，此Package才會被引用。

而利用**versionDefines**，只要這個Package裡有載入**com.walkio.game.level.common**，則會自行定義PreProcessor，在這裡是**WALKIO_LEVEL**。所以這個Package被Unity處理時順序會先是

1. versionDefines
2. defineConstraints

也就是利用這樣的寫法，其它的Package就可以引用到這裡定義的PreProcessor。然而在Assets目錄下的Script確沒有辦法拿取到這裡定義好的PreProcessor所以，利用Editor的部份，額外給予

```cs
[InitializeOnLoad]
public class ModuleDefine
{
    static ModuleDefine()
    {
        Debug.Log($"Module - Level - ModuleDefine");

        GameCommon.EditorPart.Utility.DefinesHelper.AddSymbolToAllTargets("WALKIO_LEVEL");
    }
}
```

這樣的寫法應該是每次要編譯時都會試著將這個PreProcessor加入到專案的Define裡頭，程式碼是參考PlayMaker後抽出來引用的。利用這三個不同性質的手法，讓Package被引用時就會有適當的PreProcessor同時被定義在二個地方。
