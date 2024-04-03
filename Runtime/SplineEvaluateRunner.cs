using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

namespace SplineScrubber
{
    public class SplineEvaluateRunner
    {
        public int Count => Indices.Length;
        public bool ReadyForInput { get; private set; } = true;

        public NativeList<int> Indices { get; private set; }
        public NativeArray<float3> Pos { get; private set; }
        public NativeArray<quaternion> Rotation { get; private set; }
        public NativeSpline Spline { get; set; }
        public Transform SplineTransform { get; set; }

        private NativeList<float> _ratios;

        private JobHandle _currJob;
        private bool _preparedTemp = false;

        public SplineEvaluateRunner(int capacity)
        {
            Indices = new NativeList<int>(capacity, Allocator.Persistent);
            _ratios = new NativeList<float>(capacity, Allocator.Persistent);
        }

        public void HandlePosUpdate(float t, int idx)
        {
            _ratios.Add(t);
            Indices.Add(idx);
        }

        public void Prepare()
        {
            ReadyForInput = false;
            _preparedTemp = true;
            Pos = new NativeArray<float3>(Count, Allocator.TempJob);
            Rotation = new NativeArray<quaternion>(Count, Allocator.TempJob);
        }
        
        public JobHandle Run(int batchCount = 2)
        {
            if (SplineTransform == null) //TODO Workaround - to expensive
            {
                return new JobHandle();
            }
            SplineEvaluate evaluateJob = new()
            {
                Spline = Spline,
                Ratios = _ratios.AsArray(),
                Pos = Pos,
                Rotation = Rotation,
                LocalToWorld = SplineTransform.localToWorldMatrix,
                SplineRotation = SplineTransform.rotation
            };
            _currJob = evaluateJob.Schedule(_ratios.Length, batchCount);
            return _currJob;
        }

        public void Abort()
        {
            if (!_currJob.IsCompleted)
            {
                _currJob.Complete();
            }
            
            DisposeTemp();
        }

        public void ClearAndDispose()
        {
            Indices.Clear();
            _ratios.Clear();
            DisposeTemp();
            ReadyForInput = true;
        }

        private void DisposeTemp()
        {
            if (!_preparedTemp) return;
            Pos.Dispose();
            Rotation.Dispose();
            _preparedTemp = false;
        }

        ~SplineEvaluateRunner()
        {
            _ratios.Dispose();
            Indices.Dispose();
        }
    }
}