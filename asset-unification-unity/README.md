# Project Overview

For art designer basically.

## Level part

Each **Level** will have several data to export

- Scene file
- A* pathfinding data file
- Map grid file
- Pathfind tile settings

### Scene file

This is regular Unity scene file which contains the look. It has the regular game objects that represent art visual reperesentation.

### A* pathfinding data file

This might be

- NavMesh
- Grid

depending on which works well in the zone.

This file is genreated and has the format of bytes file(binary file format).

### Map grid file

This could be either home-made image texture to show the grid defintion or use A* grid data. For now, incline to sue image texture file as A* grid data needs some further research to being to use.

### Pathfind tile settings

Each zone may have vary setup that requires diffrent grunlar of pathfind tile. Should use settings to unify the change.

## Hud Part

Three main hud for now

- App
- Preparation
- Stage

Each will be placed into a scene so that the modification can be made easily.


## 概念

在Level Editor(場景編輯器)這裡的製作是會有一組編輯時候的場景相關代表，和實際轉出後會用於遊戲中的表示物件，分別用二個package來表示

- walkio-tool-level-design
- walkio-game-level-common

場景編輯器裡會有一個代表目前所有場景資訊的部份。而在這個畫面可以快速的輸入一個新的場景的構成進而生成新的場景。

先用一個Editor Window進行產生Level的動作。此部份功能為產生

- Level ScriptableObject
- Unity Scene

- Surface Manager
- Decal Manager
- Kinematic Object Manager
- Object Pool
- Scheduler
- Audio Manager
- Spawn Point Manager
- State Manager
- Layer Manager

### Package引用

walkio-tool-level-design不論是Editor或是Runtime都只應該用於Unity Editor中，所以就特別在Walkio.Tool.Level-Design.asmdef裡加上，防止Runtime裡的MonoBehaviour不小少就進入到遊戲的加構中。

```json
{
    "defineConstraints": [
        "UNITY_EDITOR"
    ]
}
```
