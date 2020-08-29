namespace JoyBrick.Walkio.Game.Move.CrowdSimulate
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Unity.Collections;
    using Unity.Entities;
    using Unity.Mathematics;
    using UnityEngine;

    //
    public class SPHSolverBlobAssetConstructor : GameObjectConversionSystem
    {
        private static void ConvertFromAuthoringData(
            SPHSolverBlobAssetAuthoring authoring,
            BlobBuilder blobBuilder,
            ref SPHSolverBlobAsset sphSolverBlobAsset)
        {
            ref SPHSolverContext context = ref blobBuilder.Allocate(ref sphSolverBlobAsset.Context);

            Debug.Log($"SPHSolverBlobAssetConstructor - ConvertFromAuthoringData - {authoring.sphShader.ToString()}");
            Debug.Log($"SPHSolverBlobAssetConstructor - ConvertFromAuthoringData - {authoring.stiffness}");

            context.SPHShader = authoring.sphShader;
            context.KernelRadius = authoring.kernelRadius;
            context.Stiffness = authoring.stiffness;
            context.RestDensity = authoring.restDensity;
            context.Viscosity = authoring.viscosity;

            context.KPoly6Const = authoring.kPoly6Const;
            // context.Kr2 = authoring.kr2;
            // context.InverseKr9 = authoring.inverseKr9;

        }

        private void AddToEntity(BlobAssetReference<SPHSolverBlobAsset> sphSolverBlobAssetReference)
        {
            Debug.Log($"SPHSolverBlobAssetConstructor - AddToEntity");

            var crowdSimulationWorldPropertyQuery = DstEntityManager.CreateEntityQuery(
                typeof(CrowdSimulationWorld),
                typeof(CrowdSimulationWorldProperty));
            var crowdSimulationWorldEntity = crowdSimulationWorldPropertyQuery.GetSingletonEntity();
            var crowdSimulationWorldProperty = crowdSimulationWorldPropertyQuery.GetSingleton<CrowdSimulationWorldProperty>();

            Debug.Log($"SPHSolverBlobAssetConstructor - ConvertFromAuthoringData - {sphSolverBlobAssetReference.Value.Context.Value.Stiffness}");
            DstEntityManager.SetComponentData(crowdSimulationWorldEntity, new CrowdSimulationWorldProperty
            {
                SPHSolverBlobAssetRef = sphSolverBlobAssetReference
            });
        }

        protected override void OnUpdate()
        {
            var authorings =
                GetEntityQuery(typeof(SPHSolverBlobAssetAuthoring))
                    .ToComponentArray<SPHSolverBlobAssetAuthoring>();

            if (authorings.Length == 0) return;

            Debug.Log($"SPHSolverBlobAssetConstructor - OnUpdate");

            BlobAssetReference<SPHSolverBlobAsset> sphSolverBlobAssetReference;

            using (var blobBuilder = new BlobBuilder(Allocator.Temp))
            {
                ref var sphSolverBlobAsset = ref blobBuilder.ConstructRoot<SPHSolverBlobAsset>();

                var authoring = authorings[0];

                ConvertFromAuthoringData(authoring, blobBuilder, ref sphSolverBlobAsset);

                sphSolverBlobAssetReference = blobBuilder.CreateBlobAssetReference<SPHSolverBlobAsset>(Allocator.Persistent);

                //
                AddToEntity(sphSolverBlobAssetReference);
            }
        }
    }
}