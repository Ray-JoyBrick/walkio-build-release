# Project Overview

## Package use

The use of Entities 0.8.0 preview will cause the issue when build Android platform. The workaround is adding certain code to two files. This solves the issue but since the package inside Library/PackageCache is read only, the change is requried to be performed each time the project is opened. The workaround to this is to extract package folder from Library/PackageCache to somewhere else. Either put directly into Packages of each project or use manifest to specify the location

```
"com.unity.entities": "file:../../references/Unity Technology/com.unity.entities@0.8.0-preview.8"
```

This way, the modification will persist.

However this introduce another issue(only on Windows?). When the project is first checkout from git repo, opening the project will have several compile time errors related to such setup. The workaround is back to using Entites 0.8.0 preview from package manager and let it store inside Library/PackageCache. This means the workaround is circling and nothing really gets solved.

Will invesgiate more on this issue.

## ECS and MB

There are related scriptable object for settings that will be used both in ECS and MB world.

## Game Flow

1. Appwide
2. Hud
3. Environment

- Loading Hud for App
- Loading Hud for Preparation

Scene loaded

- Main Scene
- App Hud Scene
- Preparation Hud Scene
- Environment Common Scene
- Environment Specific Scene

Scripts

- Bootstrap in Game
- Entry in Game.Environment

### Main Scene

Bootstrap is here. Mainly for setting up all ECS systems.

### Environment Common Scene

All pools should reside here. This makes the loading/unloading more correct.

## Environemnt part

### Terms defined

- Grid
- Grid Cell
- Tile
- Tile Cell


PathfindBoard
FlowFieldBoardBuffer

PathfindTile
PathfindTileBuffer

### System

1. LoadEnvironmentTempalteSystem
2. LoadZoneTemplateSystem
3. GenerateZoneSystem
4. GeneratePathfindSystem

### Data

- EnvironmentData
- ZoneData

### Authoring

- GridCellDetailBlobAssetAuthoring
- GridCellIndexBlobAssetAuthoring

Very misleading name for both. One is defined as the type of grid cell in an array format. The other is defined as the index to the array.

```
[0]:
  Kind: 0
  Cost: 0
[1]:
  Kind: 1
  Cost: 255
[2]:
  Kind: 2
  Cost: 27
```

The following grid cell has the index to refer to the above

```
011000
021000
010000
000100
```

#### GridCellIndexBlobAssetAuthoring

Actually I think this might be great to be buffer instead of blob asset.

#### EnvironmentData

Has global kind of ```GridCellDetail``` apply to zones

```cs
public struct GridCellDetail
{
    public int Kind;
    public int Cost;
}
```

#### ZoneData

Can overwrite global kind of ```GridCellDetail```

### LoadZoneSystem

This primarily loads asset that defines the zone.

The zone has map grid data. This map grid data defines data for each cell.


Kind it self might be different for each zone. Suppose that for zone A,

```
[0] -> Detail A1
[1] -> Detail A2
```

but for zone B,

```
[0] -> Detail B1
[1] -> Detail B2
[2] -> Detail B3
```

After zone data is loaded, there should be data of a grid of map created.

How to set entity name, check this

- [[HowTo] ECS Entity names from GameObjects](https://forum.unity.com/threads/howto-ecs-entity-names-from-gameobjects.797739/)

```cs
EntityManager.SetName(entity, "name");
```

Make **Zone** or **Environment** entity singleton
