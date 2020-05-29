# Project Overview

For art designer basically.

## Environment part

Each **Zone** will have several data to export

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
