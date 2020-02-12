using Unity.Entities;
using UnityEngine;

public class CountUpSystem : ComponentSystem
{
    private CounterMonoBehavior _counter;
    private EntityManager _entityManager;

    protected override void OnCreate()
    {
        _counter = GameObject.FindObjectOfType<CounterMonoBehavior>();
        _entityManager = World.Active.EntityManager;
    }

    protected override void OnUpdate()
    {
        Entities.ForEach((Entity entity, ref Count count, ref CountUp countUp) =>
        {
            count.value += 1;
            _counter.SetCount(count.value);
            _entityManager.RemoveComponent<CountUp>(entity);
        });
    }
}