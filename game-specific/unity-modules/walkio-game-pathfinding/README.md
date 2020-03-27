# Module Overview

Thinking to rename the module to Navigation instead of pathfinding. As there will more than pathfinding inside this module.

```cs

// May need more info
public struct PathfindCellHolderCreationContext
{
    public int2 MapGridCellSize;
    public int2 MapGridCellCount;

    public int2 PathTileCellSize;
    public int2 PathTileCellCount;
}

public struct PathfindCellBuffer : IBufferElementData
{
    public int Value;
}

//
Entity CreatePathfindCellEntity(PathfindCellHolderCreationContext context)
{
    var archetype = EntityManager.CreateArchetype(
        typeof(PathfindCellBuffer));
    var entity = EntityManager.CreateEntity(archetype);
    var buffer = entity.AddBuffer<PathfindCellBuffer>(entity);

    var cellCount = context.PathTileCellCount.x * context.PathTileCellCount.y;

    buffer.ResizeUninitialized(cellCount);

    for (var i = 0; i < cellCount; ++i)
    {
        var index = Utility.PathTile.TileWithCellIndexToGridCellIndex();
        buffer[i] = index;
    }

    return entity;
}
```

```cs
// This context is actually formed after environment has been made to Blob Asset
public struct PathfindTileHolderCreationContext
{
    public int2 MapGridCellSize;
    public int2 MapGridCellCount;

    public int2 PathTileCellSize;
    public int2 PathTileCellCount;
}

public struct PathfindTileBuffer : IBufferElementData
{
    public Entity Value;
}

Entity CreatePathfindTileEntity(PathfindTileHolderCreationContext context)
{
    //
    var archetype = EntityManager.CreateArchetype(
        typeof(PathfindTileBuffer));
    var entity = EntityManager.CreateEntity(archetype);
    var buffer = entity.AddBuffer<PathfindTileBuffer>(entity);

    var tileCount = Utility.GridHelper.TileCount(
        context.MapGridCellSize, context.MapGridCellCount,
        context.PathTileCellSize, context.PathTileCellCount);

    //
    buffer.ResizeUninitialized(tileCount);

    for (var i = 0; i < tileCount; ++i)
    {
        var entity = CreatePathfindCellEntity(new PathfindCellHolderCreationContext
        {
            MapGridCellSize = MapGridCellSize,
            MapGridCellCount = MapGridCellCount,
            PathTileCellSize = PathTileCellSize,
            PathTileCellCount = PathTileCellCount
        });

        buffer[i] = entity;
    }
}
```

```cs
// This is used for checking if this ordered pathfind can be removed or not
public struct OrderedPathfindRecord : IComponentData
{
    public float TimeStamp;
}

//
public struct OrderedPathfindTileDetailBuffer : IBufferElementData
{
    public int Direction;
}

public struct OrderedPathfindTile : IComponentData
{
    public int TileIndex;
    public int InTileCellIndex;
    public int OutTileCellIndex;

    // public int ConnectionRelationship;
}

public struct OrderedPathfindTileBuffer : IBufferElementData
{
    public OrderedPathfindTile PathfindTile;
}

Entity CreateEntity(OrderedPathfindHolderCreationContext context)
{
    var archetype = EntityManager.CreateArchetype(
        typeof(OrderedPathfindRecord),
        typeof(OrderedPathfindTileBuffer),
        typeof(OrderedPathfindTileDetailBuffer));
    var entity = EntityManager.CreateEntity(archetype);
    var buffer = entity.AddBuffer<OrderedPathfindTileBuffer>(entity);

    var count = context.Count;

    buffer.ResizeUninitialized(count);

    for (var i = 0; i < count; ++i)
    {
        buffer[i] = new OrderedPathfindTile
        {
            TileIndex = -1,
            InTileCellIndex = -1,
            OutTileCellIndex = -1
        }
    }

    var buffer2 = entity.AddBuffer<OrderedPathfindTileDetailBuffer>(entity); 
    buffer.ResizeUninitialized(count);

    // May not be able to fiil out the detail here

    return entity;
}
```

```cs
public struct Team : IComponentData
{
    public int Id;
}

// May not need to introduce FreeTeam as no team unit suppose to be FreeTeam?
// public struct FreeTeam : IComponentData
// {
//     public int Id;
// }

public struct Unit : IComponentData
{
    public int Id;
}

public struct PathfindGroup : IComponentData
{
    // Another considersation is that the ordered path tile record may change rapidly, there will lots of
    // entity being created due to this reason. Need to device a way to reuse the cached entity. Also, may
    // need to make a rule to not alert the path entity if it is under certain condition

    // May need to play bit trick, still in plan
    // Team Id        | Depth of 4 direction split
    // 0000 0000 0000 | 0000 0000 0000 0000 0000
    public int Id;
    public Entity Value;
    public bool Ongoing;
    public int AtIndex;
}

Entity CreateEntity()
{
    var archetype = EntityManager.CreateArchetype(
        typeof(Unit),
        typeof(PathfindGroup));
    var entity = EntityManager.CreateEntity(archetype);

    return entity;
}
```