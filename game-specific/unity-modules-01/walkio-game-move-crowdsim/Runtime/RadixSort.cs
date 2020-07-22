namespace JoyBrick.Walkio.Game.Move.CrowdSim
{
    using UnityEngine;

    public class RadixSort :
        System.IDisposable
    {
        private ComputeShader _shaderRadixSort;

        private ComputeBuffer _bufferParticles;

        public const int bucketSize = 16;
        public const int bucketBitNum = 4;
        public const int sortSectionNum = 64;

        //
        private int _particleCount;
        private int _groupNum;
        private int _roundNum;
        
        public RadixSort(
            int particleCount,
            ComputeShader shaderRadixSort)
        {
            _shaderRadixSort = shaderRadixSort;
            
            //
            _particleCount = particleCount;
            _groupNum = Mathf.CeilToInt((float) _particleCount / (float) sortSectionNum);
            for (_roundNum = 1; _roundNum < 7; ++_roundNum)
            {
                if ((1 << (4 * _roundNum)) > _particleCount) break;
            }
        }

        public void Initialize(ComputeBuffer bufferParticles)
        {
            _bufferParticles = bufferParticles;
        }

        private void SortRound(int round)
        {
            
        }

        public void Sort()
        {
            for (var i = 0; i < _roundNum; ++i)
            {
                SortRound(i);
            }
        }

        public void Dispose()
        {
            _bufferParticles?.Release();
        }
    }
}
