using System;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

namespace SplineScrubber
{
    public interface ISplineEvaluate
    {
        int Count { get; }
        NativeList<int> Indices { get; }
        NativeArray<float3> Pos { get; }
        NativeArray<float3> Tan { get; }
        NativeArray<float3> Up { get; }
        
        NativeSpline Spline { get; set; }
        
        public void HandlePosUpdate(Transform target, float tPos);
        public void Prepare();
        public JobHandle Run(int batchCount);
        void ClearAndDispose();
    }
}