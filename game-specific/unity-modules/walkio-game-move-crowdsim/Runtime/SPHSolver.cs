namespace JoyBrick.Walkio.Game.Move.CrowdSim
{
    using System.Collections.Generic;
    using UnityEngine;

    // public struct Particle
    // {
    //     public float Mass;
    //     public float InverseDensity;
    //
    //     public Vector3 Position;
    //     public Vector3 Velocity;
    //
    //     public float Pressure;
    //
    //     public Particle(float mass, float inverseDensity)
    //     {
    //         //
    //         Mass = mass;
    //         InverseDensity = inverseDensity;
    //
    //         Position = Vector3.zero;
    //         Velocity = Vector3.zero;
    //         Pressure = 0;
    //     }
    //
    //     public static int stride = sizeof(float) * 27 + sizeof(int) * 2;
    // }

    public class SPHSolver : System.IDisposable
    {
        //
        private ComputeShader _shaderSPH;
        private int _kernelComputeCellIndex;
        private int _kernelUpdatePressureDensity;
        private int _kernelInitializeParticle;
        
        //
        public Vector3Int gridSize;
        public int gridCountXYZ;
        
        public ComputeBuffer _bufferParticles;
        private ComputeBuffer _bufferNeighborSpace;
        public ComputeBuffer _bufferParticleNumPerCell;
        
        private int[] _neighborSpaceInit;
        private int[] _particleNumPerCellInit;

        public const int sphThreadGroupSize = 512;
        public int sphThreadGroupNum;
        
        //
        private float _timeStep;
        private float _kernelRadius;
        private float _kr2;
        private float _inverseKr3;
        private float _inverseKr6;
        private float _inverseKr9;

        private float _stiffness;
        private float _restDensity;
        private float _viscosity;
        private float _tensionCoef;
        
        //
        private ComputeShader _shaderRadixSort;
        private RadixSort _radixSort;

        //
        private List<Particle> _particles = new List<Particle>();
        
        //
        public int CurrentParticleCount => _particles.Count;
        
        public SPHSolver(
            float timeStep,
            float kernelRadius,
            float stiffness,
            float restDensity,
            float viscosity,
            float tensionCoef,

            ComputeShader shaderSPH,
            ComputeShader shaderRadixSort)
        {
            //
            _shaderSPH = shaderSPH;
            
            //
            _kernelComputeCellIndex = _shaderSPH.FindKernel("ComputeCellIndex");
            _kernelUpdatePressureDensity = _shaderSPH.FindKernel("UpdatePressureDensity");

            //
            _timeStep = timeStep;
            _kernelRadius = kernelRadius;
            
            _kr2 = kernelRadius * kernelRadius;
            _inverseKr3 = 1 / (kernelRadius * kernelRadius * kernelRadius);
            _inverseKr6 = _inverseKr3 * _inverseKr3;
            _inverseKr9 = _inverseKr3 * _inverseKr3 * _inverseKr3;

            _stiffness = stiffness;
            _restDensity = restDensity;
            _viscosity = viscosity;
            _tensionCoef = tensionCoef;

            //
            _shaderSPH.SetFloat("_timeStep", _timeStep);
            _shaderSPH.SetFloat("_kernelRadius", _kernelRadius);
            _shaderSPH.SetFloat("_kr2", _kr2);
            _shaderSPH.SetFloat("_inverseKr3", _inverseKr3);
            _shaderSPH.SetFloat("_inverseKr6", _inverseKr6);
            _shaderSPH.SetFloat("_inverseKr9", _inverseKr9);
            _shaderSPH.SetFloat("_inverseKr9", _inverseKr9);
            _shaderSPH.SetFloat("_stiffness", _stiffness);
            _shaderSPH.SetFloat("_restDensity", _restDensity);
            _shaderSPH.SetFloat("_viscosity", _viscosity);
            _shaderSPH.SetFloat("_tensionCoef", _tensionCoef);

            //
            _shaderRadixSort = shaderRadixSort;
        }

        public void InitializeOnGPU()
        {
            //
            _radixSort = new RadixSort(CurrentParticleCount, _shaderRadixSort);
            
            //
            _bufferParticles = new ComputeBuffer(CurrentParticleCount, Particle.stride);
            _bufferParticles.SetData(_particles.ToArray());
            
            //
            _bufferNeighborSpace = new ComputeBuffer(CurrentParticleCount * 27, 4);
            _neighborSpaceInit = new int[CurrentParticleCount * 27];
            _bufferNeighborSpace.SetData(_neighborSpaceInit);
            
            //
            _bufferParticleNumPerCell = new ComputeBuffer(gridCountXYZ + 1, sizeof(int));
            _particleNumPerCellInit = new int[gridCountXYZ + 1];
            _bufferParticleNumPerCell.SetData(_particleNumPerCellInit);
            
            //
            _shaderSPH.SetBuffer(_kernelComputeCellIndex, "_ParticleCellNumPrefixSum", _bufferParticleNumPerCell);
            _shaderSPH.SetBuffer(_kernelComputeCellIndex, "_Particles", _bufferParticles);
            _shaderSPH.Dispatch(_kernelComputeCellIndex, sphThreadGroupNum, 1, 1);
            
            //
            _radixSort.Initialize(_bufferParticles);
            _radixSort.Sort();
            
            // _shaderSPH.SetInt("_ParticleNum", currParticleNum);
            
            _shaderSPH.SetBuffer(_kernelUpdatePressureDensity, "_ParticleCellNumPrefixSum", _bufferParticleNumPerCell);
            _shaderSPH.SetBuffer(_kernelUpdatePressureDensity, "_Particles", _bufferParticles);
            _shaderSPH.SetBuffer(_kernelUpdatePressureDensity, "_NeighborSpace", _bufferNeighborSpace);
            _shaderSPH.Dispatch(_kernelUpdatePressureDensity, sphThreadGroupNum, 1, 1);
            
            //
            _shaderSPH.SetBuffer(_kernelInitializeParticle, "_ParticleCellNumPrefixSum", _bufferParticleNumPerCell);
            _shaderSPH.SetBuffer(_kernelInitializeParticle, "_Particles", _bufferParticles);
            _shaderSPH.SetBuffer(_kernelInitializeParticle, "_NeighborSpace", _bufferNeighborSpace);
            _shaderSPH.Dispatch(_kernelInitializeParticle, sphThreadGroupNum, 1, 1);

        }

        public void StepOnGPU()
        {
            _shaderSPH.SetBuffer(_kernelUpdatePressureDensity, "_Particles", _bufferParticles);
            _shaderSPH.Dispatch(_kernelUpdatePressureDensity, sphThreadGroupNum, 1, 1);
        }


        public void Dispose()
        {
            //
            _bufferParticles?.Release();
            _bufferNeighborSpace?.Release();
            _bufferParticleNumPerCell?.Release();
            
            //
            _radixSort?.Dispose();
        }
    }
}
