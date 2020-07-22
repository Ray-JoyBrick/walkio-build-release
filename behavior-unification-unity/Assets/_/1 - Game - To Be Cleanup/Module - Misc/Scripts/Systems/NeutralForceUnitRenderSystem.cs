namespace Game
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
    public class NeutralForceUnitRenderSystem : SystemBase
    {
        public Camera SceneCamera { get; set; }
        public Mesh UnitMesh { get; set; }
        public Material UnitMaterial { get; set; }
        
        protected override void OnUpdate()
        {
            var materialPropertyBlock = new MaterialPropertyBlock();

            // var entityQuery = GetEntityQuery(typeof(Unit), typeof(Translation), typeof(Rotation));
            // var entityQuery = GetEntityQuery(typeof(Unit), typeof(LocalToWorld));
            var entityQuery = GetEntityQuery(new EntityQueryDesc
            {
                All = new ComponentType[] { typeof(Unit), typeof(LocalToWorld) },
                None = new ComponentType[] { typeof(Leader) }
            });
            // var translations = entityQuery.ToComponentDataArray<Translation>(Allocator.TempJob);
            // var rotations = entityQuery.ToComponentDataArray<Rotation>(Allocator.TempJob);
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
            
            // var matrices = new List<Matrix4x4>();
            // for (var i = 0; i < localToWorlds.Length; ++i)
            // {
            //     var localToWorld = localToWorlds[i];
            //     // var translation = translations[i];
            //     // var rotation = rotations[i];
            //     // var matrix = Matrix4x4.TRS(translation.Value, rotation.Value, Vector3.one);
            //     var matrix = Matrix4x4.TRS(localToWorld.Position, localToWorld.Rotation, Vector3.one);
            //     // var matrix = (Matrix4x4) localToWorlds[i].Value;
            //     
            //     // Debug.Log($"matrix: {matrix}");
            //     
            //     matrices.Add(matrix);
            // }
            //
            // Graphics.DrawMeshInstanced(
            //     UnitMesh,
            //     0,
            //     UnitMaterial,
            //     matrices,
            //     materialPropertyBlock
            //     );

        //     Entities
        //         .WithAll<Unit>()
        //         .ForEach((Entity entity, Translation translation, Rotation rotation) =>
        //         {
        //             var matrix = Matrix4x4.TRS(translation.Value, rotation.Value, Vector3.one);
        //             Graphics.DrawMeshInstanced(
        //                 UnitMesh,
        //                 
        //                 // translation.Value,
        //                 // rotation.Value,
        //                 UnitMaterial,
        //                 matrices,
        //                 materialPropertyBlock,
        //                 ShadowCastingMode.Off);
        //         })
        //         .WithoutBurst()
        //         .Run();

            if (localToWorlds.IsCreated)
            {
                localToWorlds.Dispose();
            }
        }
    }
}
