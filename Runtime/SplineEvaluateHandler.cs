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

        private NativeArray<float> _times;
        
        private readonly List<float> _timesList = new(1000);

        public void Prepare()
        {
            var count = Transforms.Count;
            _times = new NativeArray<float>(count, Allocator.TempJob);
            Pos = new NativeArray<float3>(count, Allocator.TempJob);
            Tan = new NativeArray<float3>(count, Allocator.TempJob);
            Up = new NativeArray<float3>(count, Allocator.TempJob);
            _times.CopyFrom(_timesList.ToArray()); //TODO inefficient due to list to array?
        }

        public void ScheduleEvaluate(Transform target, float tPos)
        {
            _timesList.Add(tPos);
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
            return evaluateJob.Schedule(_timesList.Count, batchCount);
        }
        
        public void ClearAndDispose()
        {
            Transforms.Clear();
            _timesList.Clear();
            _times.Dispose();
            Pos.Dispose();
            Tan.Dispose();
            Up.Dispose();
        }
    }
}