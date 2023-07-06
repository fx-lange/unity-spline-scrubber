using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Jobs;
using UnityEngine.Splines;
using UnityEngine.UIElements;

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
        public void Execute(int index, TransformAccess transform)
        {
            var rotation = Quaternion.LookRotation(Tan[index], Up[index]);
            transform.SetPositionAndRotation(Pos[index], rotation);
        }
    }

    [BurstCompile]
    public struct CollectResultsJob : IJobParallelFor
    {
        [ReadOnly] public NativeList<int> Indices;
        [ReadOnly] public NativeArray<float3> PosIn;
        [ReadOnly] public NativeArray<float3> TanIn;
        [ReadOnly] public NativeArray<float3> UpIn;

        [WriteOnly] public NativeArray<float3> Pos;
        [WriteOnly] public NativeArray<float3> Tan;
        [WriteOnly] public NativeArray<float3> Up;

        public void Execute(int index)
        {
            int mappedIdx = Indices[index];
            Pos[mappedIdx] = PosIn[index];
            Tan[mappedIdx] = TanIn[index];
            Up[mappedIdx] = UpIn[index];
        }
    }
}