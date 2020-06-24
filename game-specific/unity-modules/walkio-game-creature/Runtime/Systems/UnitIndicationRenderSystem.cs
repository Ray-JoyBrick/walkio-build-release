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
        public Camera SceneCamera { get; set; }
        public Mesh UnitMesh { get; set; }
        public Material UnitMaterial { get; set; }
        
        protected override void OnUpdate()
        {
            var materialPropertyBlock = new MaterialPropertyBlock();

            var entityQuery = GetEntityQuery(new EntityQueryDesc
            {
                All = new ComponentType[] { typeof(UnitIndication), typeof(LocalToWorld) },
                None = new ComponentType[] { typeof(Leader) }
            });

            var localToWorlds = entityQuery.ToComponentDataArray<LocalToWorld>(Allocator.TempJob);

            // Debug.Log($"localToWorlds.Length: {localToWorlds.Length}");

            var sliceCount = 1023;
            for (var i = 0; i < localToWorlds.Length; i += sliceCount)
            {
                var sliceSize = math.min(localToWorlds.Length - i, sliceCount);

                var matrices = new List<Matrix4x4>();
                for (var j = 0; j < sliceSize; ++j)
                {
                    var localToWorld = localToWorlds[i + j];
                    var matrix = Matrix4x4.TRS(localToWorld.Position, localToWorld.Rotation, Vector3.one);
                    matrices.Add(matrix);
                }

                Graphics.DrawMeshInstanced(
                    UnitMesh,
                    0,
                    UnitMaterial,
                    matrices,
                    materialPropertyBlock,
                    ShadowCastingMode.Off,
                    false,
                    0,
                    SceneCamera
                );
            }
            
            if (localToWorlds.IsCreated)
            {
                localToWorlds.Dispose();
            }
        }
    }
}
