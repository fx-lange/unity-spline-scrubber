using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

namespace SplineScrubber
{
    public class SplineEvaluateHandler
    {
        public readonly List<Transform> Transforms = new(1000);

        public NativeArray<float3> Pos;
        public NativeArray<float3> Tan;
        public NativeArray<float3> Up;

        private NativeList<float> _times;

        public SplineEvaluateHandler()
        {
            _times = new NativeList<float>(1000,Allocator.Persistent);
        }
        
        public void Prepare()
        {
            var count = Transforms.Count;
            // _times = new NativeArray<float>(count, Allocator.TempJob);
            Pos = new NativeArray<float3>(count, Allocator.TempJob);
            Tan = new NativeArray<float3>(count, Allocator.TempJob);
            Up = new NativeArray<float3>(count, Allocator.TempJob);
            // _times.CopyFrom(_timesList.ToArray());
        }

        public void ScheduleEvaluate(Transform target, float tPos)
        {
            _times.Add(tPos);
            Transforms.Add(target);
        }

        public JobHandle Run(NativeSpline spline, int batchCount = 2)
        {
            SplineEvaluate evaluateJob = new()
            {
                Spline = spline,
                ElapsedTimes = _times,
                Pos = Pos,
                Tan = Tan,
                Up = Up
            };
            return evaluateJob.Schedule(_times.Length, batchCount);
        }
        
        public void ClearAndDispose()
        {
            Transforms.Clear();
            _times.Clear();
            Pos.Dispose();
            Tan.Dispose();
            Up.Dispose();
        }

        ~SplineEvaluateHandler()
        {
            _times.Dispose();
        }
    }
}