namespace JoyBrick.Walkio.Game
{
    using System;
    using System.Collections.Generic;
    using Unity.Entities;
    using Unity.Mathematics;
    using Unity.Rendering;
    using Unity.Transforms;
    using UnityEngine;
    using UnityEngine.Tilemaps;

    public struct WorldMap : IComponentData
    {
        public int Width;
        public int Height;
    }
    
    public struct MapCell : IBufferElementData
    {
        public int Value;
        public static implicit operator int(MapCell v) => v.Value;
        public static implicit operator MapCell(int v) => new MapCell {Value = v};
        
    }

    public struct PathTile : IBufferElementData
    {
        public int2 Value;
        public static implicit operator int2(PathTile v) => v.Value;
        public static implicit operator PathTile(int2 v) => new PathTile {Value = v};
        
    }

    [DisableAutoCreation]
    public class EnvironmentSystem : SystemBase
    {
        private IAssetLoadingService _assetLoadingService;
        
        //
        private EntityArchetype _mapArchetype;
        private EntityArchetype _pathArchetype;
        
        //
        private GameObject _mapPrefab;
        private GameObject _gridPrefab;
        
        protected override void OnCreate()
        {
            base.OnCreate();
            
            var als = World.GetOrCreateSystem<AssetLoadingSystem>();
            _assetLoadingService = als as IAssetLoadingService;
            
            //
            _mapArchetype = EntityManager.CreateArchetype(
                typeof(WorldMap),
                typeof(MapCell));

            _pathArchetype = EntityManager.CreateArchetype(
                typeof(PathTile));

            //
            _assetLoadingService.LoadAssetAsync<GameObject>("Test Map", (mapPrefab) =>
            {
                //
                _assetLoadingService.LoadAssetAsync<GameObject>("Test Grid", (gridPrefab) =>
                {
                    //
                    _mapPrefab = mapPrefab;
                    _gridPrefab = gridPrefab;
                    
                    var worldMapInstance = GameObject.Instantiate(mapPrefab);
                    var gridInstance = GameObject.Instantiate(gridPrefab);
                    
                    // After grid instance is fetched, should create entity out of it for later use
                    CreateWorldMapAndPathTile(gridInstance);

                    GameObject.Destroy(worldMapInstance);
                    GameObject.Destroy(gridInstance);
                });
            });
        }

        private void CreateWorldMapAndPathTile(GameObject gridInstance)
        {
            var entity = EntityManager.CreateEntity(_mapArchetype);
            var mapCellBuffer = EntityManager.GetBuffer<MapCell>(entity);
            
            var maxX = 0.0f;
            var maxY = 0.0f;
            
            // var maxTileCount = 0;
            
            foreach (Transform child in gridInstance.transform)
            {
                var tileMap = child.GetComponent<Tilemap>();
                var tileMapSize = tileMap.size;
            
                if (string.Compare(tileMap.tag, "World Map Base", StringComparison.Ordinal) == 0)
                {
                    // ++maxTileCount;
                    
                    for (var x = 0; x < tileMapSize.x; ++x)
                    {
                        for (var y = 0; y < tileMapSize.y; ++y)
                        {
                            var cellLocation = new Vector3Int(x, y, 0);
                            var tile = tileMap.GetTile(cellLocation);
                            if (tile != null)
                            {
                                var worldPos = tileMap.CellToWorld(cellLocation);
                                if (maxX < worldPos.x)
                                {
                                    maxX = worldPos.x;
                                }
            
                                if (maxY < worldPos.z)
                                {
                                    maxY = worldPos.z;
                                }
                            }
                        }
                    }
                }
            
                // Debug.Log($"Tilemap: {tileMap.name}, size: {tileMapSize}");
            }
            
            var totalSize = ((int)maxX + 1) * ((int)maxY + 1);
            
            Debug.Log($"maxX: {maxX} maxY: {maxY} totalsize: {totalSize}");
            
            var cells = new int[totalSize];
            Array.Clear(cells, 0, cells.Length);
            foreach (Transform child in gridInstance.transform)
            {
                var tileMap = child.GetComponent<Tilemap>();
                var tileMapSize = tileMap.size;
            
                if (string.Compare(tileMap.tag, "World Map Obstacle", StringComparison.Ordinal) == 0)
                {
                    var basePos = tileMap.CellToWorld(tileMap.origin);
                    
                    for (var x = 0; x < tileMapSize.x; ++x)
                    {
                        for (var y = 0; y < tileMapSize.y; ++y)
                        {
                            var cellLocation = new Vector3Int(x, y, 0);
                            var tile = tileMap.GetTile(cellLocation);
                            if (tile != null)
                            {
                                var worldPos = tileMap.CellToWorld(cellLocation);
            
                                var index = (int)worldPos.x * (int)maxY + (int)worldPos.z;
                            
                                cells[index] = 1;
                                
                                Debug.Log($"index: {index} is obstacle at pos: {worldPos}");
                            }
                        }
                    }
                }
            }

            mapCellBuffer.ResizeUninitialized(totalSize);
            
            for (var i = 0; i < totalSize; ++i)
            {
                mapCellBuffer[i] = cells[i];
            }
            
            // Has to make it here in order to avoid issue
            var pathTileEntity = EntityManager.CreateEntity(_pathArchetype);
            var pathTileBuffer = EntityManager.GetBuffer<PathTile>(pathTileEntity);
            
            var xCount = ((int) maxX + 1) / 10;
            var yCount = ((int) maxY + 1) / 10;
            var maxTileCount = xCount * yCount;
            
            Debug.Log($"maxTileCount: {maxTileCount}");

            pathTileBuffer.ResizeUninitialized(maxTileCount);
            
            for (var i = 0; i < maxTileCount; ++i)
            {
                var startIndex = i * 100;
                var endIndex = startIndex + (100 - 1);
                pathTileBuffer[i] = new int2(startIndex, endIndex);
            }
        }

        private void CreatePathTile(GameObject gridInstance)
        {
            var entity = EntityManager.CreateEntity(_pathArchetype);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            _assetLoadingService.Release(_mapPrefab);
            _assetLoadingService.Release(_gridPrefab);
        }

        protected override void OnUpdate()
        {
        }
    }    
}
