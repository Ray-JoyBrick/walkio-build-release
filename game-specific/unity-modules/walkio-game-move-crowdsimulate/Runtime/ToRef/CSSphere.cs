namespace JoyBrick.Walkio.Game.Move.CrowdSimulate
{
    using System.Numerics;

    public struct CSSphere
    {
        public Vector3 center;
        public float radius;
        public Vector3 velocity;
        public int active;

        public CSSphere(Vector3 center, float radius, Vector3 velocity, int active)
        {
            this.center = center;
            this.radius = radius;
            this.velocity = velocity;
            this.active = active;
        }

        public static int stride { get { return 7 * sizeof(float) + sizeof(int); } }
    }
}
