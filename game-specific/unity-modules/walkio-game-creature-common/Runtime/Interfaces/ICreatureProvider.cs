namespace JoyBrick.Walkio.Game.Creature
{
    using UniRx;
    using UnityEngine;
    
    //
    public class MinionData
    {
        public Mesh mesh;
        public Material material;
    }

    public interface ICreatureProvider
    {
        void AddTeamLeaderNpcPrefab(GameObject prefab);

        void AddTeamMinionPrefab(GameObject prefab);
        void AddTeamMinionData(MinionData minionData);

        void CreateTeamLeaderNpcAt(Vector3 location);
        void CreateTeamLeaderPlayerAt(Vector3 location);
        
        //
        GameObject GetTeamMinionPrefab(int index);
        int GetMinionDataCount { get; }
        MinionData GetMinionDataByIndex(int index);
        
    }
}
