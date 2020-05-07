namespace JoyBrick.Walkio.Game
{
    using HellTap.PoolKit;
    using UnityEngine;

    public partial class Bootstrap :
        IPoolKitListener
    {
        private void CreateTeamLeaderFromPool()
        {
            var pool = PoolKit.Find("Team Leader Pool");
            if (pool != null)
            {
                // Should create from factory and give location later
                var randomPosition = new Vector3(
                    Random.Range(0, 100.0f),
                    0,
                    Random.Range(0, 100.0f));
                pool.Spawn("Character_BusinessMan_Shirt_01", randomPosition, Quaternion.identity);
            }
        }
        
        public void OnSpawn(Pool pool)
        {
        }

        public void OnDespawn()
        {
        }
    }
}
