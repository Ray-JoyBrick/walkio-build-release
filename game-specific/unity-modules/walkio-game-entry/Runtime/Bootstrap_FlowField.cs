namespace JoyBrick.Walkio.Game
{
    using System.Collections.Generic;
    using System.Linq;
    using HellTap.PoolKit;
    using Pathfinding;
    using UniRx;
    using Unity.Entities;
    using Unity.Mathematics;
    using UnityEngine;

    //
#if WALKIO_MOVE_FLOWFIELD
    using GameMoveFlowField = JoyBrick.Walkio.Game.Move.FlowField;
#endif

    public partial class Bootstrap

#if WALKIO_MOVE_FLOWFIELD
        : GameMoveFlowField.IFlowFieldWorldProvider
#endif

    {
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.BoxGroup("Flow Field")]
#endif
        public ScriptableObject flowFieldWorldData;

        public ScriptableObject FlowFieldWorldData => flowFieldWorldData;

        public Entity FlowFieldWorldEntity { get; set; }

        public GameObject FlowFieldTileBlobAssetAuthoringPrefab { get; set; }

        //
        private EntityArchetype _pathPointFoundEventEntityArchetype;

        private void SetupFlowFieldPart()
        {
            _pathPointFoundEventEntityArchetype = _entityManager.CreateArchetype(
                typeof(GameMoveFlowField.PathPointFound),
                typeof(GameMoveFlowField.PathPointFoundProperty),
                typeof(GameMoveFlowField.PathPointSeparationBuffer),
                typeof(GameMoveFlowField.PathPointBuffer));
        }

        public void CalculateLeadingTilePath(int forWhichGroup, float3 targetPosition, List<float3> positions)
        {
            var pool = PoolKit.Find("Seeker Pool");
            if (pool == null)
            {
                _logger.Warning($"Bootstrap - CalculateLeadingTilePath - No pool found");
                return;
            }

            var spawned = pool.Spawn("Seeker", Vector3.zero, Quaternion.identity);
            if (spawned == null)
            {
                _logger.Warning($"Bootstrap - CalculateLeadingTilePath - Can not spawn");
                return;
            }

            var seeker = spawned.GetComponent<Pathfinding.Seeker>();
            if (seeker == null)
            {
                _logger.Warning($"Bootstrap - CalculateLeadingTilePath - No seeker component found to be used");
                pool.Despawn(spawned);
                return;
            }

            seeker.StartMultiTargetPath(positions.Select(p => (Vector3)p).ToArray(), (Vector3)targetPosition, true, path =>
            {
                if (path.error)
                {
                    _logger.Error(path.errorLog);

                    // What to do if encountering error? As callback does not get called, is this going to impact
                    // flow field move part?
                }
                else
                {
                    if (path is MultiTargetPath mtp)
                    {
                        // Instead of making callback, create an event entity to notify that the finding is done
                        // for ECS might be a better approach to do the async operation
                        var separationIndices =
                            mtp.vectorPaths.Aggregate(new List<int2> { new int2(0, 0) }, (acc, next) =>
                            {
                                var baseIndex = acc.Last();
                                var index = new int2( baseIndex.y, baseIndex.y + next.Count - 1);
                                acc.Add(index);

                                return acc;
                            });

                        separationIndices.RemoveAt(0);

                        var combinedPoints =
                            mtp.vectorPaths.Aggregate(new List<Vector3>(), (acc, next) =>
                            {
                                acc.AddRange(next);

                                return acc;
                            });

                        var pathPointFoundEventEntity = _entityManager.CreateEntity(_pathPointFoundEventEntityArchetype);

#if UNITY_EDITOR
                        _entityManager.SetName(pathPointFoundEventEntity, $"Path Point Found Event - Group {forWhichGroup}");
#endif
                        _entityManager.SetComponentData(pathPointFoundEventEntity, new GameMoveFlowField.PathPointFoundProperty
                        {
                            GroupId = forWhichGroup
                        });

                        var pathPointSeparationBuffer =
                            _entityManager.AddBuffer<GameMoveFlowField.PathPointSeparationBuffer>(
                                pathPointFoundEventEntity);
                        pathPointSeparationBuffer.ResizeUninitialized(separationIndices.Count);
                        for (var i = 0; i < separationIndices.Count; ++i)
                        {
                            pathPointSeparationBuffer[i] = separationIndices[i];
                        }

                        var pathPointBuffer = _entityManager.AddBuffer<GameMoveFlowField.PathPointBuffer>(pathPointFoundEventEntity);
                        pathPointBuffer.ResizeUninitialized(combinedPoints.Count);

                        for (var i = 0; i < combinedPoints.Count; ++i)
                        {
                            var convertedPoint = (float3) combinedPoints[i];

                            // _logger.Warning($"Bootstrap - CalculateLeadingTilePath - convertedPoint: {convertedPoint}");
                            pathPointBuffer[i] = convertedPoint;
                        }

                        _assistants?.ForEach(assistant =>
                        {
                            assistant?.ShowPoints(forWhichGroup, combinedPoints, 3.0f);
                        });
                    }
                }

                // Observable.Timer(System.TimeSpan.FromMilliseconds(UnityEngine.Random.Range(30000, 40000)))
                Observable.Timer(System.TimeSpan.FromMilliseconds(UnityEngine.Random.Range(200, 1000)))
                    .Subscribe(_ =>
                    {
                        pool.Despawn(spawned);
                    })
                    .AddTo(_compositeDisposable);
            });
        }
    }
}
