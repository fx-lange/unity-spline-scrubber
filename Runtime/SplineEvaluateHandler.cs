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

        public int Count => Indices.Length;
        
        public NativeArray<float3> Pos;
        public NativeArray<float3> Tan;
        public NativeArray<float3> Up;
        public NativeList<int> Indices;

        private NativeList<float> _times;

        public SplineEvaluateHandler()
        {
            Indices = new NativeList<int>(1000, Allocator.Persistent); //Todo magic number
            _times = new NativeList<float>(1000,Allocator.Persistent); //todo magic number
        }
        
        public void Prepare()
        {
            // using (PrepareMarker.Auto())
            {
                Pos = new NativeArray<float3>(Count, Allocator.TempJob);
                Tan = new NativeArray<float3>(Count, Allocator.TempJob);
                Up = new NativeArray<float3>(Count, Allocator.TempJob);
            }
        }

        public void ScheduleEvaluate(int idx, float tPos)
        {
            // using (ScheduleMarker.Auto())
            {
                _times.Add(tPos);
                Indices.Add(idx);
            }
        }

        public JobHandle Run(NativeSpline spline, int batchCount = 2)
        {
            // EvaluateMarker.Begin();
            
            SplineEvaluate evaluateJob = new()
            {
                Spline = spline,
                ElapsedTimes = _times.AsArray(),
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
            Indices.Clear();
            _times.Clear();
            Pos.Dispose();
            Tan.Dispose();
            Up.Dispose();
        }

        ~SplineEvaluateHandler()
        {
            _times.Dispose();
            Indices.Dispose();
        }
    }
}