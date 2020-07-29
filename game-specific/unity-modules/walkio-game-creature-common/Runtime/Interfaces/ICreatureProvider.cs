namespace JoyBrick.Walkio.Game.Creature
{
    using UnityEngine;

    public interface ICreatureProvider
    {
        void AddTeamLeaderNpcPrefab(GameObject prefab);

        void CreateTeamLeaderNpcAt(Vector3 location);
        void CreateTeamLeaderPlayerAt(Vector3 location);
    }
}
