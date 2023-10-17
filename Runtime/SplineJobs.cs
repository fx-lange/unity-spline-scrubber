using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Jobs;
using UnityEngine.Splines;

namespace SplineScrubber
{
    [BurstCompile]
    public struct SplineEvaluateTransform : IJobParallelForTransform
    {
        [ReadOnly] public NativeArray<float> ElapsedTimes;
        [ReadOnly] public NativeSpline Spline;

        public void Execute(int index, TransformAccess transform)
        {
            Spline.Evaluate(ElapsedTimes[index], out float3 pos, out float3 tan, out float3 up);
            var rotation = Quaternion.LookRotation(tan, up);
            transform.SetPositionAndRotation(pos, rotation);
        }
    }

    [BurstCompile]
    public struct SplineEvaluate : IJobParallelFor
    {
        [ReadOnly] public NativeArray<float> ElapsedTimes;
        [ReadOnly] public NativeSpline Spline;

        public NativeArray<float3> Pos;
        public NativeArray<float3> Tan;
        public NativeArray<float3> Up;

        public void Execute(int index)
        {
            Spline.Evaluate(ElapsedTimes[index], out float3 position, out float3 tangent, out float3 up);
            Pos[index] = position;
            Tan[index] = tangent;
            Up[index] = up;
        }
    }
    
    [BurstCompile]
    public struct UpdateTransforms : IJobParallelForTransform
    {
        [ReadOnly] public NativeArray<float3> Pos;
        [ReadOnly] public NativeArray<float3> Tan;
        [ReadOnly] public NativeArray<float3> Up;
        [ReadOnly] public NativeArray<bool> IgnoreRotation;
        public void Execute(int index, TransformAccess transform)
        {
            if (IgnoreRotation[index])
            {
                transform.position = Pos[index];
                return;
            }
            
            var rotation = Quaternion.LookRotation(Tan[index], Up[index]);
            transform.SetPositionAndRotation(Pos[index], rotation);
        }
    }
    
    [BurstCompile]
    public struct UpdatePositions : IJobParallelForTransform
    {
        [ReadOnly] public NativeArray<float3> Pos;
        public void Execute(int index, TransformAccess transform)
        {
            transform.position = Pos[index];
        }
    }

    [BurstCompile]
    public struct CollectResultsJob : IJob
    {
        [ReadOnly] public NativeList<int> Indices;
        [ReadOnly] public NativeArray<float3> PosIn;
        [ReadOnly] public NativeArray<float3> TanIn;
        [ReadOnly] public NativeArray<float3> UpIn;

        [ReadOnly] public int Length;

        [WriteOnly] public NativeArray<float3> Pos;
        [WriteOnly] public NativeArray<float3> Tan;
        [WriteOnly] public NativeArray<float3> Up;

        public void Execute()
        {
            for (int i = 0; i < Length; i++)
            {
                int mappedIdx = Indices[i];
                Pos[mappedIdx] = PosIn[i];
                Tan[mappedIdx] = TanIn[i];
                Up[mappedIdx] = UpIn[i];
            }
        }
    }
}