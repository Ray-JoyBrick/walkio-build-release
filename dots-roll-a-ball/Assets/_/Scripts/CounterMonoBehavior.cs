using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;

public class CounterMonoBehavior : MonoBehaviour
{
    public Text countText;

    private void Awake()
    {
        // _countText = this.GetComponent<Text>();

        var entityManager = World.Active.EntityManager;
        entityManager.CreateEntity(typeof(Count));
    }

    public void SetCount(int count)
    {
        countText.text = $"Count: {count}";
    }
}
