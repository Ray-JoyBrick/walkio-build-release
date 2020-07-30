namespace JoyBrick.Walkio.Game.Creature
{
    using System.Collections;
    using System.Collections.Generic;
    using UniRx;
    using Unity.Collections;
    using Unity.Entities;
    using Unity.Mathematics;
    using Unity.Transforms;
    using UnityEngine;
    using UnityEngine.Rendering;
    
    using GameLevel = JoyBrick.Walkio.Game.Level;

#if WALKIO_FLOWCONTROL
    using GameFlowControl = JoyBrick.Walkio.Game.FlowControl;
#endif
    
#if WALKIO_FLOWCONTROL
    [GameFlowControl.DoneSettingAssetWait("Stage")]
#endif
    [DisableAutoCreation]
    public class PresentUnitIndicationSystem : SystemBase
    {
        private static readonly UniRx.Diagnostics.Logger _logger = new UniRx.Diagnostics.Logger(nameof(PresentUnitIndicationSystem));

        //
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();
        
        //
        // public Camera SceneCamera { get; set; }
        // public List<Mesh> UnitMeshs { get; set; }
        // public List<Material> UnitMaterials { get; set; }

#if WALKIO_FLOWCONTROL
        public GameFlowControl.IFlowControl FlowControl { get; set; }
#endif
        public ICreatureProvider CreatureProvider { get; set; }
        public GameLevel.ILevelPropProvider LevelPropProvider { get; set; }

        //
        private EntityQuery _entityQuery;
        
        //
        private readonly Dictionary<int, List<int>> _cachedCounts = new Dictionary<int, List<int>>();

        //
        private const int SliceCount = 1023;

        private bool _canUpdate;
        
            
        public void Construct()
        {
#if WALKIO_FLOWCONTROL
            //
            FlowControl?.AssetSettingStarted
                .Where(x => x.Name.Contains("Stage"))
                .Subscribe(x =>
                {
                    _logger.Debug($"Module - Creature - PresentUnitIndicationSystem - Construct - Receive SettingAsset");

                    // _canUpdate = true;
                    
                    for (var i = 0; i < CreatureProvider.GetMinionDataCount; ++i)
                    {
                        _cachedCounts.Add(i, new List<int>());
                    }
                })
                .AddTo(_compositeDisposable);
            
            FlowControl?.FlowReadyToStart
                .Where(x => x.Name.Contains("Stage"))
                .Subscribe(x =>
                {
                    _logger.Debug($"Module - Creature - PresentUnitIndicationSystem - Construct - Receive FlowReadyToStart");
                    _canUpdate = true;
                })
                .AddTo(_compositeDisposable);            
#endif

        }

        protected override void OnCreate()
        {
            base.OnCreate();
            
            _entityQuery = GetEntityQuery(new EntityQueryDesc
            {
                All = new ComponentType[] { typeof(UnitIndication), typeof(LocalToWorld) },
                None = new ComponentType[] { typeof(Leader) }
            });
            
            RequireForUpdate(_entityQuery);
        }

        private static void CheckAnyMatrixNaN(in Matrix4x4 m)
        {
            // for (var i = 0; i < 16; ++i)
            // {
            //     if (float.IsNaN(m[i]))
            //     {
            //         Debug.Log($"Matrix value at [{i}] is NaN");
            //     }
            // }
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

                    CheckAnyMatrixNaN(matrix);
                    
                    matrices.Add(matrix);
                }

                var camera = LevelPropProvider.LevelCamera;
                
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
                    camera
                );
            }
        }
        
        //
        protected override void OnUpdate()
        {
            // _logger.Debug($"Module - Creature - PresentUnitIndicationSystem - Update - _canUpdate: {_canUpdate}");

            if (!_canUpdate) return;

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
                    // _logger.Debug($"Module - Creature - PresentUnitIndicationSystem - Update - indication count: {unitIndications.Length}");

                    //
                    for (var i = 0; i < unitIndications.Length; ++i)
                    {
                        var unitIndication = unitIndications[i];
                        _cachedCounts[unitIndication.Kind].Add(i);
                    }

                    foreach (var pair in _cachedCounts)
                    {
                        var minionData = CreatureProvider.GetMinionDataByIndex(pair.Key);

                        var mesh = minionData.mesh;
                        var material = minionData.material;
                        
                        // _logger.Debug($"Module - Creature - PresentUnitIndicationSystem - Update - mesh: {mesh} material: {material}");


                        UpdateEachKind(pair.Value, in localToWorlds, materialPropertyBlock, mesh, material);
                    }
                }
            }
        }
        
        protected override void OnDestroy()
        {
            base.OnDestroy();

            _compositeDisposable?.Dispose();
        }
    }
}
