namespace JoyBrick.Walkio.Game.Move.CrowdSimulate
{
    using System.Collections;

    public class CSParticleComparer : IComparer
    {
        public int Compare(object x, object y)
        {
            CSParticle px = (CSParticle)x;
            CSParticle py = (CSParticle)y;
            if (px.cellIdx1d < py.cellIdx1d)
                return -1;
            else if (px.cellIdx1d > py.cellIdx1d)
                return 1;
            else return 0;
        }

        public static CSParticleComparer comparerInst = new CSParticleComparer();
    }
}
