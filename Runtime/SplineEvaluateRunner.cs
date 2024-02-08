using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine.Splines;

namespace SplineScrubber
{
    public class SplineEvaluateRunner
    {
        public int Count => Indices.Length;
        public bool ReadyForInput { get; private set; } = true;

        public NativeList<int> Indices { get; private set; }
        public NativeArray<float3> Pos { get; private set; }
        public NativeArray<float3> Tan { get; private set; }
        public NativeArray<float3> Up { get; private set; }
        public NativeSpline Spline { get; set; }

        private NativeList<float> _ratios;

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
            Pos = new NativeArray<float3>(Count, Allocator.TempJob);
            Tan = new NativeArray<float3>(Count, Allocator.TempJob);
            Up = new NativeArray<float3>(Count, Allocator.TempJob);
        }
        
        public JobHandle Run(int batchCount = 2)
        {
            SplineEvaluate evaluateJob = new()
            {
                Spline = Spline,
                Ratios = _ratios.AsArray(),
                Pos = Pos,
                Tan = Tan,
                Up = Up
            };
            return evaluateJob.Schedule(_ratios.Length, batchCount);
        }

        public void ClearAndDispose()
        {
            Indices.Clear();
            _ratios.Clear();
            Pos.Dispose();
            Tan.Dispose();
            Up.Dispose();

            ReadyForInput = true;
        }

        ~SplineEvaluateRunner()
        {
            _ratios.Dispose();
            Indices.Dispose();
        }
    }
}