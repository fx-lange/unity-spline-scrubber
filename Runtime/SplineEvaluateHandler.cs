using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

namespace SplineScrubber
{
    public class SplineEvaluateHandler
    {
        public int Count => Indices.Length;

        public NativeList<int> Indices { get; private set; }
        public NativeArray<float3> Pos { get; private set; }
        public NativeArray<float3> Tan { get; private set; }
        public NativeArray<float3> Up { get; private set; }
        public NativeSpline Spline { get; set; }

        private NativeList<float> _times;
        private readonly SplinesMoveHandler _moveHandler;
        private bool _readyForInput = true;

        public SplineEvaluateHandler(SplinesMoveHandler moveHandler)
        {
            _moveHandler = moveHandler;
            _moveHandler.Register(this);
            
            Indices = new NativeList<int>(_moveHandler.Capacity, Allocator.Persistent);
            _times = new NativeList<float>(_moveHandler.Capacity, Allocator.Persistent);
        }

        public void HandlePosUpdate(Transform target, float tPos)
        {
            if (!_readyForInput)
            {
                return; //can happen during drag clip
            }
            
            var idx = _moveHandler.Schedule(target);
            _times.Add(tPos);
            Indices.Add(idx);
        }

        public void Prepare()
        {
            _readyForInput = false;
            Pos = new NativeArray<float3>(Count, Allocator.TempJob);
            Tan = new NativeArray<float3>(Count, Allocator.TempJob);
            Up = new NativeArray<float3>(Count, Allocator.TempJob);
        }
        
        public JobHandle Run(int batchCount = 2)
        {
            SplineEvaluate evaluateJob = new()
            {
                Spline = Spline,
                ElapsedTimes = _times.AsArray(),
                Pos = Pos,
                Tan = Tan,
                Up = Up
            };
            return evaluateJob.Schedule(_times.Length, batchCount);
        }

        public void ClearAndDispose()
        {
            Indices.Clear();
            _times.Clear();
            Pos.Dispose();
            Tan.Dispose();
            Up.Dispose();

            _readyForInput = true;
        }

        ~SplineEvaluateHandler()
        {
            _times.Dispose();
            Indices.Dispose();
        }
    }
}