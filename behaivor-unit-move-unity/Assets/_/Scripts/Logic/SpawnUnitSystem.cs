namespace JoyBrick.Walkio.Game
{
    using Unity.Entities;
    using Unity.Mathematics;
    using Unity.Transforms;

    [DisableAutoCreation]
    public class SpawnUnitSystem : SystemBase
    {
        //
        private EntityArchetype _unitArchetype;

        private Random _rnd;
        
        private int _count = 0;
        private const int MaxCount = 10;
        
        protected override void OnCreate()
        {
            base.OnCreate();
            
            _rnd = new Unity.Mathematics.Random((uint) System.DateTime.UtcNow.Ticks);

            _unitArchetype = EntityManager.CreateArchetype(
                typeof(LocalToWorld),
                typeof(Translation),
                
                typeof(Unit),
                typeof(UnitMoveToTarget));
        }

        protected override void OnUpdate()
        {
            if (_count >= MaxCount)
            {
                return;
            }

            var entity = EntityManager.CreateEntity(_unitArchetype);

            //
            EntityManager.AddComponentData<Translation>(entity, new Translation
            {
                Value = float3.zero
            });

            EntityManager.AddComponent<Unit>(entity);

            var randomTarget = _rnd.NextFloat3(new float3(-30.0f, 0, -30.0f), new float3(30.0f, 0, 30.0f));
            EntityManager.AddComponentData(entity, new UnitMoveToTarget
            {
                Target = randomTarget,
                MoveSpeed = 1.0f
            });

            ++_count;
        }
    }
}
