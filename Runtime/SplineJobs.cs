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
            // var pos = Spline.EvaluatePosition(ElapsedTime);
            // var tan = Spline.EvaluateTangent(ElapsedTime);
            // var up = Vector3.up;
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
}