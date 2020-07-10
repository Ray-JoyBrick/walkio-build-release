namespace JoyBrick.Walkio.Game.Creature
{
    using System.Collections;
    using System.Collections.Generic;
    using Unity.Collections;
    using Unity.Entities;
    using Unity.Mathematics;
    using Unity.Transforms;
    using UnityEngine;
    using UnityEngine.Rendering;

    [DisableAutoCreation]
    public class UnitIndicationRenderSystem : SystemBase
    {
        //
        public Camera SceneCamera { get; set; }
        public List<Mesh> UnitMeshs { get; set; }
        public List<Material> UnitMaterials { get; set; }

        //
        private EntityQuery _entityQuery;
        
        //
        private readonly Dictionary<int, List<int>> _cachedCounts = new Dictionary<int, List<int>>();

        //
        private const int SliceCount = 1023;
            
        public void Construct()
        {
            for (var i = 0; i < UnitMeshs.Count; ++i)
            {
                _cachedCounts.Add(i, new List<int>());
            }
        }

        protected override void OnCreate()
        {
            base.OnCreate();
            
            _entityQuery = GetEntityQuery(new EntityQueryDesc
            {
                All = new ComponentType[] { typeof(UnitIndication), typeof(LocalToWorld) },
                None = new ComponentType[] { typeof(Leader) }
            });
        }

        private void UpdateEachKind(
            List<int> indices,
            in NativeArray<LocalToWorld> localToWorlds,
            MaterialPropertyBlock materialPropertyBlock,
            Mesh mesh,
            Material material)
        {
            // Group by count 1023 and send to DrawMeshInstanced
            for (var i = 0; i < indices.Count; i += SliceCount)
            {
                var sliceSize = math.min(indices.Count - i, SliceCount);

                var matrices = new List<Matrix4x4>();
                for (var j = 0; j < sliceSize; ++j)
                {
                    var actualIndex = indices[i + j];
                    var localToWorld = localToWorlds[actualIndex];
                    var matrix = Matrix4x4.TRS(localToWorld.Position, localToWorld.Rotation, Vector3.one);
                    matrices.Add(matrix);
                }

                Graphics.DrawMeshInstanced(
                    mesh,
                    0,
                    material,
                    matrices,
                    materialPropertyBlock,
                    // ShadowCastingMode.Off,
                    ShadowCastingMode.On,
                    // false,
                    true,
                    0,
                    SceneCamera
                );
            }
        }
        
        //
        protected override void OnUpdate()
        {
            //
            for (var i = 0; i < _cachedCounts.Count; ++i)
            {
                _cachedCounts[i].Clear();
            }

            //
            var materialPropertyBlock = new MaterialPropertyBlock();

            using (var unitIndications = _entityQuery.ToComponentDataArray<UnitIndication>(Allocator.TempJob))
            {
                using (var localToWorlds = _entityQuery.ToComponentDataArray<LocalToWorld>(Allocator.TempJob))
                {
                    //
                    for (var i = 0; i < unitIndications.Length; ++i)
                    {
                        var unitIndication = unitIndications[i];
                        _cachedCounts[unitIndication.Kind].Add(i);
                    }

                    foreach (var pair in _cachedCounts)
                    {
                        var mesh = UnitMeshs[pair.Key];
                        var material = UnitMaterials[pair.Key];

                        UpdateEachKind(pair.Value, in localToWorlds, materialPropertyBlock, mesh, material);
                    }
                }
            }
        }
    }
}
