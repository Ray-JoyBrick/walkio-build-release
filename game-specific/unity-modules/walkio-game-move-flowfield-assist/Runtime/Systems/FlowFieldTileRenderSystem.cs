namespace JoyBrick.Walkio.Game.Move.FlowField.Assist
{
    using System.Collections.Generic;
    using UniRx;
    using Unity.Entities;
    using Unity.Mathematics;
    using Unity.Physics;
    using Unity.Transforms;
    using UnityEngine;
    using UnityEngine.Rendering;

    //
    using GameCommon = JoyBrick.Walkio.Game.Common;
    using GameMove = JoyBrick.Walkio.Game.Move;
    using Material = UnityEngine.Material;

    [DisableAutoCreation]
    public class FlowFieldTileRenderSystem : SystemBase
    {
        //
        private static readonly UniRx.Diagnostics.Logger _logger = new UniRx.Diagnostics.Logger(nameof(FlowFieldTileRenderSystem));

        //
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();
        
        //
        private EntityQuery _entityQuery;

        //
        private const int SliceCount = 1023;

        //
        private bool _canUpdate;

        private int _tileCellCount = 1;

        private Mesh _mesh = null;
        private Material _material;

        private Vector2Int _segmentCount;
        private Vector3 _offset;
        
        //
        public Camera SceneCamera { get; set; }        
        public GameCommon.IFlowControl FlowControl { get; set; }
        public GameCommon.ISceneAssistProvider SceneAssistProvider { get; set; }
        
        public void Construct()
        {
            _logger.Debug($"FlowFieldTileRenderSystem - Construct");

            //
            FlowControl.AllDoneSettingAsset
                .Where(x => x.Name.Contains("Stage"))
                .Subscribe(x =>
                {
                    _logger.Debug($"FlowFieldTileRenderSystem - Construct - Receive AllDoneSettingAsset");
                    _canUpdate = true;
                })
                .AddTo(_compositeDisposable);

            //
            _segmentCount = SceneAssistProvider.SegmentCount;
            _offset = SceneAssistProvider.StartOffset;
            _material = SceneAssistProvider.PlanMaterial;
        }        

        protected override void OnCreate()
        {
            base.OnCreate();
            
            _entityQuery = GetEntityQuery(new EntityQueryDesc
            {
                All = new ComponentType[]
                {
                    typeof(GameCommon.LevelSetting)
                }
            });
        }

        protected override void OnUpdate()
        {
            if (!_canUpdate) return;
            
            //
            var levelSettingEntity = _entityQuery.GetSingletonEntity();
            var levelSetting = EntityManager.GetComponentData<GameCommon.LevelSetting>(levelSettingEntity);
            var flowFieldBuffer = EntityManager.GetBuffer<GameCommon.FlowFieldTileBuffer>(levelSettingEntity);
            
            //
            if (_mesh == null)
            {
                var loadFlowFieldSystem =
                    World.DefaultGameObjectInjectionWorld.GetExistingSystem<GameMove.FlowField.LoadFlowFieldSystem>();
            
                var settingDataAsset = loadFlowFieldSystem.SettingDataAsset;
                var settingData = settingDataAsset as GameMove.FlowField.Template.SettingData;
                if (settingData != null)
                {
                    _tileCellCount = settingData.tileCellCount;
                }
                
                var xSize = _tileCellCount;
                var zSize = _tileCellCount;
                var xSegments = _segmentCount.x;
                var zSegments = _segmentCount.y;
                
                _logger.Debug($"FlowFieldTileRenderSystem - OnUpdate - _segmentCount: {_segmentCount}");
            
                var draftMesh = ProceduralToolkit.MeshDraft.Plane(xSize, zSize, xSegments, zSegments);
                _mesh = draftMesh.ToMesh();
            }
            
            var hTileCount = levelSetting.HorizontalCellCount / _tileCellCount;
            var vTileCount = levelSetting.VerticalCellCount / _tileCellCount;
            
            // Material property block is now just created, may assign some properties later
            var materialPropertyBlock = new MaterialPropertyBlock();
            
            // Group by count 1023 and send to DrawMeshInstanced
            for (var i = 0; i < flowFieldBuffer.Length; i += SliceCount)
            {
                var sliceSize = math.min(flowFieldBuffer.Length - i, SliceCount);
            
                var matrices = new List<Matrix4x4>();
                for (var j = 0; j < sliceSize; ++j)
                {
                    // var actualIndex = flowFieldBuffer[i + j];
                    // var localToWorld = localToWorlds[actualIndex];
                    var combinedIndex = i + j;
            
                    var zPosIndex = combinedIndex / hTileCount;
                    var xPosIndex = combinedIndex % hTileCount;
            
                    var zPos = zPosIndex * _tileCellCount;
                    var xPos = xPosIndex * _tileCellCount;
                    
                    var position = (new Vector3(xPos, 0, zPos)) + _offset;
                    // var position = Vector3.zero;
                    var rotation = Quaternion.identity;
                    var matrix = Matrix4x4.TRS(position, rotation, Vector3.one);
                    matrices.Add(matrix);
                }
            
                Graphics.DrawMeshInstanced(
                    _mesh,
                    0,
                    _material,
                    matrices,
                    materialPropertyBlock,
                    ShadowCastingMode.Off,
                    false,
                    0,
                    SceneCamera
                );
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            _compositeDisposable?.Dispose();
        }
    }
}
