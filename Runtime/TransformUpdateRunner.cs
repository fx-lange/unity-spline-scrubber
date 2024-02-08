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
        
        private NativeArray<float3> _pos;
        private NativeArray<quaternion> _rotation;

        private bool _prepared = false;
        
        public void Init(int capacity)
        {
            _transformsAccess = new TransformAccessArray(capacity);
        }

        public void Schedule(Transform target)
        {
            _transformsAccess.Add(target);
        }

        public JobHandle PrepareMove(int count, List<SplineEvaluateRunner> evaluateRunners, JobHandle dependency)
        {
            _pos = new NativeArray<float3>(count, Allocator.TempJob);
            _rotation = new NativeArray<quaternion>(count, Allocator.TempJob);
            
            foreach (var evaluateRunner in evaluateRunners)
            {
                CollectResultsJob collectJob = new()
                {
                    Indices = evaluateRunner.Indices,
                    PosIn = evaluateRunner.Pos,
                    RotateIn = evaluateRunner.Rotation,
                    Length = evaluateRunner.Count,
                    Pos = _pos,
                    Rotation = _rotation,
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
                Rotation = _rotation
            };

            return transformJob.Schedule(_transformsAccess, dependency);
        }

        public void Dispose()
        {
            _transformsAccess.Dispose();
            if (!_prepared)
            {
                return;
            }
            
            _pos.Dispose();
            _rotation.Dispose();

            _prepared = false;
        }
    }
}