namespace JoyBrick.Walkio.Game
{
    using Unity.Entities;

    public class SpawnUnitSystem : SystemBase
    {
        private EntityArchetype _unitArchetype;

        private int _count = 0;
        private const int MaxCount = 10;
        
        protected override void OnCreate()
        {
            base.OnCreate();

            _unitArchetype = EntityManager.CreateArchetype(
                typeof(Unit));
        }

        protected override void OnUpdate()
        {
            if (_count >= MaxCount)
            {
                return;
            }

            var entity = EntityManager.CreateEntity(_unitArchetype);

            EntityManager.AddComponent<Unit>(entity);

            ++_count;
        }
    }
}
