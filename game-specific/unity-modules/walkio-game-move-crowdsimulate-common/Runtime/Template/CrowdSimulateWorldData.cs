namespace JoyBrick.Walkio.Game.Move.CrowdSimulate.Template
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "Crowd Simulate World Data", menuName = "Walkio/Game/Crowd Simulate/Crowd Simulate World Data")]
    public class CrowdSimulateWorldData : ScriptableObject
    {
        public float kernelRadius;
        public float stiffness;
        public float restDensity;
        public float viscosity;

        public float kPoly6Const;
        public float lapkViscosityConst;
        public float gradKSpikyConst;
    }
}
