namespace JoyBrick.Walkio.Game
{
    using System.Collections.Generic;
    using UnityEngine;

    public interface IBootstrapAssistant
    {
        void ShowPoints(int groupId, List<Vector3> points, float timeInSeconds);
    }
}
