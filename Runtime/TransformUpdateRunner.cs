using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Jobs;

namespace SplineScrubber
{
    public class TransformUpdateRunner
    {
        private TransformAccessArray _transformsAccess;
        private NativeList<bool> _ignoreRotation;
        
        private NativeArray<float3> _pos;
        private NativeArray<float3> _tan;
        private NativeArray<float3> _up;

        private bool _prepared = false;
        
        public void Init(int capacity)
        {
            _transformsAccess = new TransformAccessArray(capacity);
            _ignoreRotation = new NativeList<bool>(capacity, Allocator.Persistent);
        }

        public void Schedule(Transform target, bool ignoreRotation)
        {
            _transformsAccess.Add(target);
            _ignoreRotation.Add(ignoreRotation);
        }

        public JobHandle PrepareMove(int count, List<SplineEvaluateRunner> evaluateRunners, JobHandle dependency)
        {
            _pos = new NativeArray<float3>(count, Allocator.TempJob);
            _tan = new NativeArray<float3>(count, Allocator.TempJob);
            _up = new NativeArray<float3>(count, Allocator.TempJob);
            
            foreach (var evaluateRunner in evaluateRunners)
            {
                CollectResultsJob collectJob = new()
                {
                    Indices = evaluateRunner.Indices,
                    PosIn = evaluateRunner.Pos,
                    TanIn = evaluateRunner.Tan,
                    UpIn = evaluateRunner.Up,
                    Length = evaluateRunner.Count,
                    Pos = _pos,
                    Tan = _tan,
                    Up = _up
                };
                dependency = collectJob.Schedule(dependency);
            }

            _prepared = true;
            return dependency;
        }

        public JobHandle RunMove(JobHandle dependency)
        {
            UpdateTransforms transformJob = new()
            {
                Pos = _pos,
                Tan = _tan,
                Up = _up,
                IgnoreRotation = _ignoreRotation.AsArray()
            };

            return transformJob.Schedule(_transformsAccess, dependency);
        }

        public void Dispose()
        {
            _transformsAccess.Dispose();
            _ignoreRotation.Dispose();
            if (!_prepared)
            {
                return;
            }
            
            _pos.Dispose();
            _tan.Dispose();
            _up.Dispose();

            _prepared = false;
        }
    }
}