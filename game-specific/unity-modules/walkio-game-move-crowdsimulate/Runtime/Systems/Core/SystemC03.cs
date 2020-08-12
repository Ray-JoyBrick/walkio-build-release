namespace JoyBrick.Walkio.Game.Move.CrowdSimulate
{
    using System.Collections.Generic;
    using System.Linq;
    using UniRx;
    using Unity.Collections;
    using Unity.Entities;
    using Unity.Mathematics;
    using Unity.Physics;
    using Unity.Transforms;
    using UnityEngine;
    using GameMove = JoyBrick.Walkio.Game.Move;

    //
#if WALKIO_FLOWCONTROL
    using GameFlowControl = JoyBrick.Walkio.Game.FlowControl;
#endif

    [DisableAutoCreation]
    public class SystemC03 : SystemBase
    {
        private static readonly UniRx.Diagnostics.Logger _logger = new UniRx.Diagnostics.Logger(nameof(SystemC03));

        //
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();

        //
        private BeginInitializationEntityCommandBufferSystem _entityCommandBufferSystem;

        //
#if WALKIO_FLOWCONTROL
        public GameFlowControl.IFlowControl FlowControl { get; set; }
#endif

        //
        private bool _canUpdate;
        private EntityQuery _particleEntityQuery;

        public void Construct()
        {
            _logger.Debug($"Module - Move - CrowdSimulate - SystemC03 - Construct");

#if WALKIO_FLOWCONTROL
            //
            FlowControl?.FlowReadyToStart
                .Where(x => x.Name.Contains("Stage"))
                .Subscribe(x =>
                {
                    _logger.Debug($"Module - Move - CrowdSimulate - SystemC03 - Construct - Receive AllDoneSettingAsset");
                    _canUpdate = true;
                })
                .AddTo(_compositeDisposable);
#endif
        }

        protected override void OnCreate()
        {
            base.OnCreate();

            _entityCommandBufferSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
            
            _particleEntityQuery = GetEntityQuery(new EntityQueryDesc
            {
                All = new ComponentType[]
                {
                    ComponentType.ReadOnly<Particle>(),
                    ComponentType.ReadOnly<ParticleProperty>(),
                    ComponentType.ReadOnly<GameMove.MoveByForce>(),
                    typeof(NearbyParticleBuffer)
                }
            });

            RequireForUpdate(_particleEntityQuery);
        }

        protected override void OnUpdate()
        {
            var commandBuffer = _entityCommandBufferSystem.CreateCommandBuffer();
            var concurrentCommandBuffer = commandBuffer.AsParallelWriter();

            //
            var deltaTime = Time.DeltaTime;

            Entities
                .WithAll<Particle>()
                .ForEach((Entity entity, ref Translation translation, ref ParticleProperty particleProperty, ref Rotation rotation) =>
                {
                    var acceleration = particleProperty.Force;
                    // var deltaTime = 0.1f;

                    // if (acceleration.sqrMagnitude > (_maxAcceleration * _maxAcceleration))
                    // {
                    //     acceleration = acceleration.normalized * _maxAcceleration;
                    // }

                    var accX = Mathf.Clamp(acceleration.x, -20.0f, 20.0f);
                    var accZ = Mathf.Clamp(acceleration.z, -20.0f, 20.0f);
            
                    acceleration.x = accX;
                    acceleration.z = accZ;
            
                    particleProperty.Velocity = particleProperty.Velocity + (acceleration * deltaTime);
                    translation.Value = translation.Value + (particleProperty.Velocity * deltaTime);

                    // gameObject.transform.position = _position;
                    // gameObject.transform.forward = _velocity.normalized;

                    var adjustedDirection = math.normalize(particleProperty.Velocity);
                    
                    var smoothedRotation = math.slerp(
                        rotation.Value,
                        quaternion.LookRotationSafe(adjustedDirection, math.up()), 1f - math.exp(-deltaTime));
                    rotation.Value = smoothedRotation;
                })
                .WithoutBurst()
                .Run();

            _entityCommandBufferSystem.AddJobHandleForProducer(Dependency);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            _compositeDisposable?.Dispose();
        }
    }
}

// namespace Game
// {
//     using System;
//     using System.Collections;
//     using System.Collections.Generic;
//     using Cinemachine;
//     using UnityEngine;
//
//     [System.Serializable]
//     public class Agent
//     {
//         public GameObject gameObject;
//         
//         private Vector3 _position;
//         private float _radius;
//         private Vector3 _velocity;
//
//         private float _prefSpeed;
//         private Vector3 _prefVelocity;
//
//         private List<Agent> _nearbyAgents = new List<Agent>();
//
//         public Vector3 Position => _position;
//
//         public float Radius => _radius;
//         public Vector3 Velocity => _velocity;
//
//         public List<Agent> NearbyAgents => _nearbyAgents;
//
//         private float _k;
//         private float _t0;
//         private float _m;
//         private float _ksi;
//
//         private Vector3 _force;
//
//         private Vector3 _goal;
//
//         private float _maxAcceleration;
//         
//         public void Initialize(float k, float t0, float m, float ksi, float speed)
//         {
//             _k = k;
//             _t0 = t0;
//             _m = m;
//             _ksi = ksi;
//
//             _maxAcceleration = 20.0f;
//
//             _prefSpeed = speed;
//             
//             _position = gameObject.transform.position;
//             _radius = 0.5f;
//             _force = Vector3.zero;
//             _velocity = Vector3.forward;
//             
//             _goal = new Vector3(UnityEngine.Random.Range(-80.0f, 80.0f), 0, UnityEngine.Random.Range(-80.0f, 80.0f));
//         }
//
//         public void UpdateGoal(Vector3 goal)
//         {
//             _goal = goal;
//         }
//
//         public void AdjustSpeed(float speed)
//         {
//             _prefSpeed = speed;
//         }
//
//         public void UpdatePosition()
//         {
//             var acceleration = _force;
//             // var deltaTime = 0.1f;
//
//             // if (acceleration.sqrMagnitude > (_maxAcceleration * _maxAcceleration))
//             // {
//             //     acceleration = acceleration.normalized * _maxAcceleration;
//             // }
//
//             var accX = Mathf.Clamp(acceleration.x, -_maxAcceleration, _maxAcceleration);
//             var accZ = Mathf.Clamp(acceleration.z, -_maxAcceleration, _maxAcceleration);
//             
//             acceleration.x = accX;
//             acceleration.z = accZ;
//             
//             var deltaTime = Time.deltaTime;
//             _velocity = _velocity + (acceleration * deltaTime);
//             _position = _position + (_velocity * deltaTime);
//
//             gameObject.transform.position = _position;
//             gameObject.transform.forward = _velocity.normalized;
//             // gameObject.GetComponent<Rigidbody>().velocity = _velocity;
//             // Debug.Log($"Agent - UpdatePosition - _velocity: {_velocity} _position: {_position}");
//         }
//
//         public void UpdateNearbyAgents(IEnumerable<Agent> nearbyAgents)
//         {
//             _nearbyAgents.Clear();
//             _nearbyAgents.AddRange(nearbyAgents);
//         }
//         
//         public void UpdateOneStep()
//         {
//             _prefVelocity = _goal - _position;
//             var distanceToGoalSquare = _prefVelocity.sqrMagnitude;
//             var distanceToGoal = _prefVelocity.magnitude;
//             
//             // _prefVelocity *= ()
//
//             _prefVelocity *= (_prefSpeed / distanceToGoal);
//             
//             ComputeForces();
//         }
//
//         private void ComputeForces()
//         {
//             _force = (_prefVelocity - _velocity) / _ksi;
//             
//             var nearbyAgentCount = _nearbyAgents.Count;
//             for (var i = 0; i < nearbyAgentCount; ++i)
//             {
//                 var otherAgent = _nearbyAgents[i];
//
//                 var distanceToOtherAgentSquare = (otherAgent.Position - Position).sqrMagnitude;
//                 var radiusOfBothAgent = (otherAgent.Radius + Radius);
//                 var radiusOfBothAgentSquare = (radiusOfBothAgent * radiusOfBothAgent);
//
//                 if (this != otherAgent && distanceToOtherAgentSquare != radiusOfBothAgentSquare)
//                 {
//                     // Debug.Log($"Agent - ComputeForces - distanceToOtherAgentSquare: {distanceToOtherAgentSquare}");
//                     
//                     if (distanceToOtherAgentSquare < radiusOfBothAgentSquare)
//                     {
//                         var distanceToOtherAgent = (otherAgent.Position - Position).magnitude;
//                         var recalculateLength = ((otherAgent.Radius + Radius) - distanceToOtherAgent);
//                         radiusOfBothAgentSquare = (recalculateLength * recalculateLength);
//                     }
//
//                     var w = otherAgent.Position - Position;
//                     var v = Velocity - otherAgent.Velocity;
//
//                     var a = Vector3.Dot(v, v);
//                     var b = Vector3.Dot(w, v);
//                     var c = Vector3.Dot(w, w) - radiusOfBothAgentSquare;
//
//                     var discr = (b * b) - (a * c);
//                     if ((discr > Mathf.Epsilon) && (Mathf.Abs(a) > Mathf.Epsilon))
//                     {
//                         var discrSquareRoot = Mathf.Sqrt(discr);
//                         var t = ((b - discrSquareRoot) / a);
//                         if (t > Mathf.Epsilon)
//                         {
//                             _force += (
//                                 (-_k * Mathf.Exp(-t / _t0))
//                                 * (v - (b * v - a * w) / discrSquareRoot)
//                                 / (a * Mathf.Pow(t, _m))
//                                 * (_m / t + 1.0f / _t0)
//                             );
//                         }
//                     }
//                 }
//             }
//             
//             // Debug.Log($"Agent - ComputeForces - _force: {_force}");
//         }
//     }
//     
//     public class Bootstrap : MonoBehaviour
//     {
//         public int agentCount = 50;
//         public List<GameObject> agentPrefabs;
//         public List<Agent> agents;
//
//         public int groupCount = 4;
//         public float agentSpeed = 1.0f;
//         public Vector3 maxGoalBound;
//         public Vector3 minGoalBound;
//
//         public CinemachineVirtualCamera virtualCamera;
//         
//         private void Start()
//         {
//             for (var i = 0; i < agentCount; ++i)
//             {
//                 var goalIndex = (i % agentPrefabs.Count);
//
//                 var agentPrefab = agentPrefabs[goalIndex];
//                 var position = new Vector3(
//                     UnityEngine.Random.Range(-30.0f, 30.0f),
//                     0,
//                     UnityEngine.Random.Range(-30.0f, 30.0f));
//                 var createdAgent = GameObject.Instantiate(agentPrefab, position, Quaternion.identity);
//                 var agent = new Agent();
//
//                 
//                 agent.gameObject = createdAgent;
//
//                 if (i == 0)
//                 {
//                     virtualCamera.Follow = agent.gameObject.transform;
//                     virtualCamera.LookAt = agent.gameObject.transform;
//                 }
//                 
//                 agents.Add(agent);
//             }
//             
//             for (var i = 0; i < agents.Count; ++i)
//             {
//                 var agent = agents[i];
//                 
//                 agent.Initialize(1.5f, 3.0f, 1.0f, 0.54f, agentSpeed);
//             }
//
//             StartCoroutine(TimedUpdateGoal());
//         }
//
//         IEnumerator TimedUpdateGoal()
//         {
//             while (true)
//             {
//                 yield return new WaitForSeconds(10.0f);
//
//                 var goals = new List<Vector3>();
//                 for (var i = 0; i < groupCount; ++i)
//                 {
//                     var goal = new Vector3(
//                         UnityEngine.Random.Range(minGoalBound.x, maxGoalBound.x),
//                         0,
//                         UnityEngine.Random.Range(minGoalBound.z, maxGoalBound.z));
//                     goals.Add(goal);
//                 }
//
//                 for (var i = 0; i < agents.Count; ++i)
//                 {
//                     var agent = agents[i];
//
//                     var goalIndex = (i % groupCount);
//                     // var goalIndex = 0;
//                     var goal = goals[goalIndex];
//                     agent.AdjustSpeed(UnityEngine.Random.Range(0.5f, agentSpeed * 4.0f));
//                     agent.UpdateGoal(goal);
//                 }
//             }
//         }
//
//         private void Update()
//         {
//             for (var i = 0; i < agents.Count; ++i)
//             {
//                 var agent = agents[i];
//                 var nearbyAgents = CollectNearbyAgents(agent);
//                 agent.UpdateNearbyAgents(nearbyAgents);
//                 agent.UpdateOneStep();
//             }
//
//             for (var i = 0; i < agents.Count; ++i)
//             {
//                 var agent = agents[i];
//                 
//                 agent.UpdatePosition();
//             }
//         }
//
//         private IEnumerable<Agent> CollectNearbyAgents(Agent agent)
//         {
//             var nearbyAgents = new List<Agent>();
//             for (var i = 0; i < agents.Count; ++i)
//             {
//                 var otherAgent = agents[i];
//                 if (otherAgent != agent)
//                 {
//                     var nearby = (otherAgent.Position - agent.Position).sqrMagnitude < 400.0f;
//                     if (nearby)
//                     {
//                         nearbyAgents.Add(otherAgent);
//                     }
//                 }
//             }
//             
//             return nearbyAgents;
//         }
//     }
// }
