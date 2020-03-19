# Module Overview

This modules is solely responsible for managing environment affair.

How world is represented should be separated from how pathfinding data is represented. For example, suppose the world map is defined by

- width: 20
- height: 30

and

- tileWidth: 10
- tileHeight: 10

This will form a very nice pathfinding tile data.

However, if the world map is

- width: 32
- height: 46

and

- tileWidth: 12
- tileHeight: 9

Then forming the tile data will be a bit tricky. As for the calculation, there will be

- horizontal tile count: 3
- vertical tile count: 6

Which will be ```3 * 6 = 18``` tiles. But there will be tiles that have no mapped world map info, which need to assign out boundary data for it.

### GameObjectConversionSystem

This system although derived from ComponentSystem, it is at different system group. It will take effect whenever there is proper type to be converted.

For now, if ScriptableObject needs to be converted into BlobAsset, it has to go through authoring part.

```cs
public class SOFacadeAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public ScriptableObjectAsset asset;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        // dstManager.AddComponentData(entity, asset);
        dstManager.AddComponentObject(entity, asset);
    }
}
```

In this case, there are different consideration to use ScriptableObject then converting by the help of authoring approach or just use authoring gameobject with necessary data.


Scriptable can be used with

```cs
EntityManager.AddComponentObject
```

in this usage, there won't be warning about constructor being called from new.

Can not use ```AssetReference``` to let addressable to load itself as it is loaded in async manner which will have very short time it is null reference. The correct loading will be

1. Load ScriptableObject
2. Load Prefab(for authoring use)
3. Assign ScriptableObject onto Prefab
4. Instantiate Prefab

After instantiating prefab, the conversion system will kick in automatically

1. Conversion system starts to convert
2. Convert is done
3. Remove entity to clean up

### What to load

1. Image for grid layout
2. ScriptableObject for grid cell data
