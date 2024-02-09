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
        [ReadOnly] public NativeArray<float> Ratios;
        [ReadOnly] public NativeSpline Spline;
        [ReadOnly] public float4x4 LocalToWorld;
        [ReadOnly] public quaternion LocalToWorldRotation;

        public NativeArray<float3> Pos;
        public NativeArray<quaternion> Rotation;

        public void Execute(int index)
        {
            Spline.Evaluate(Ratios[index], out float3 position, out float3 tangent, out float3 up);
            Pos[index] = math.transform(LocalToWorld, position);
            Rotation[index] = math.mul(LocalToWorldRotation,quaternion.LookRotation(tangent, up));
        }
    }
    
    [BurstCompile]
    public struct UpdateTransforms : IJobParallelForTransform
    {
        [ReadOnly] public NativeArray<float3> Pos;
        [ReadOnly] public NativeArray<quaternion> Rotation;
        public void Execute(int index, TransformAccess transform)
        {
            transform.SetPositionAndRotation(Pos[index], Rotation[index]);
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
        [ReadOnly] public NativeArray<quaternion> RotateIn;

        [ReadOnly] public int Length;

        [WriteOnly] public NativeArray<float3> Pos;
        [WriteOnly] public NativeArray<quaternion> Rotation;

        public void Execute()
        {
            for (int i = 0; i < Length; i++)
            {
                int mappedIdx = Indices[i];
                Pos[mappedIdx] = PosIn[i];
                Rotation[mappedIdx] = RotateIn[i];
            }
        }
    }
}