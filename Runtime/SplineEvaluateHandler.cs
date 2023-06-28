using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.Splines;

namespace SplineScrubber
{
    public class SplineEvaluateHandler
    {
        static readonly ProfilerMarker ScheduleMarker = new (ProfilerCategory.Scripts,"SplineEvaluateHandler.Schedule"); 
        static readonly ProfilerMarker PrepareMarker = new (ProfilerCategory.Scripts,"SplineEvaluateHandler.Prepare");
        static readonly ProfilerMarker EvaluateMarker = new (ProfilerCategory.Scripts,"SplineEvaluateHandler.Run");

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
            // using (PrepareMarker.Auto())
            {
                var count = Transforms.Count;
                Pos = new NativeArray<float3>(count, Allocator.TempJob);
                Tan = new NativeArray<float3>(count, Allocator.TempJob);
                Up = new NativeArray<float3>(count, Allocator.TempJob);
            }
        }

        public void ScheduleEvaluate(Transform target, float tPos)
        {
            // using (ScheduleMarker.Auto())
            {
                _times.Add(tPos);
                Transforms.Add(target);
            }
        }

        public JobHandle Run(NativeSpline spline, int batchCount = 2)
        {
            // EvaluateMarker.Begin();
            
            SplineEvaluate evaluateJob = new()
            {
                Spline = spline,
                ElapsedTimes = _times,
                Pos = Pos,
                Tan = Tan,
                Up = Up
            };
            var jobHandle = evaluateJob.Schedule(_times.Length, batchCount);
            
            // EvaluateMarker.End();
            
            return jobHandle;
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