using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.Jobs;
using UnityEngine.Splines;
using Transform = UnityEngine.Transform;

namespace SplineScrubber
{
    public class SplinesMoveHandler : MonoBehaviour, ISplineJobHandler
    {
        static readonly ProfilerMarker ScheduleMarker = new (ProfilerCategory.Scripts,"SplinesMoveHandler.Schedule"); 
        static readonly ProfilerMarker EvaluateMarker = new (ProfilerCategory.Scripts,"SplinesMoveHandler.Evaluate");
        static readonly ProfilerMarker MoveMarker = new (ProfilerCategory.Scripts, "SplinesMoveHandler.Move");

        private class JobData
        {
            public NativeSpline Spline;
            public readonly SplineEvaluateHandler Handler = new();
        }

        private Dictionary<SplineClipData, JobData> _mapping = new();
        private int _targetCount = 0;
        private TransformAccessArray _transformsAccess;
        private JobHandle _updateTransformHandle;

        private NativeArray<JobHandle> _handles;
        private NativeArray<float3> _pos;
        private NativeArray<float3> _tan;
        private NativeArray<float3> _up;

        private void OnEnable()
        {
            //TODO link (private run) into post director
        }

        private void OnDisable()
        {
            //TODO unlink
        }

        private void LateUpdate()
        {
            RunLate();
            DisposeAndClear();
        }

        public void UpdatePos(Transform target, float tPos, SplineClipData spline)
        {
            ScheduleMarker.Begin();
            
            if (!_mapping.TryGetValue(spline, out var jobData)) 
            {
                jobData = new JobData()
                {
                    Spline = spline.NativeSpline
                };
                _mapping[spline] = jobData;
            }
            jobData.Handler.ScheduleEvaluate(target, tPos);
            _targetCount++;
            
            ScheduleMarker.End();
        }

        public void Run()
        {
            EvaluateMarker.Begin();
            //run all jobs
            var jobCount = _mapping.Count;
            _handles = new NativeArray<JobHandle>(jobCount, Allocator.TempJob);
            var transforms = new Transform[_targetCount];

            int jobIdx = 0;
            int cpyIndex = 0;
            foreach (var pair in _mapping)
            {
                var jobData = pair.Value;
                jobData.Handler.Prepare();
                
                _handles[jobIdx] = jobData.Handler.Run(jobData.Spline);
                jobData.Handler.Transforms.CopyTo(transforms, cpyIndex);
                
                cpyIndex += jobData.Handler.Transforms.Count;
                jobIdx++;
            }

            //TODO combine must be a job or we have to wait for complete here
            var combined = JobHandle.CombineDependencies(_handles);
            combined.Complete();
            EvaluateMarker.End();
            MoveMarker.Begin();
            int count = _targetCount;
            _transformsAccess = new TransformAccessArray(transforms);
            _pos = new NativeArray<float3>(count, Allocator.TempJob);
            _tan = new NativeArray<float3>(count, Allocator.TempJob);
            _up = new NativeArray<float3>(count, Allocator.TempJob);
            int index = 0;
            foreach (var pair in _mapping)
            {
                var jobData = pair.Value;
                var jobPos = jobData.Handler.Pos;
                var length = jobPos.Length;
                var posSub = _pos.GetSubArray(index, length);
                jobPos.CopyTo(posSub);
                var jobTan = jobData.Handler.Tan;
                var tanSub = _tan.GetSubArray(index, jobTan.Length);
                jobTan.CopyTo(tanSub);
                var jobUp = jobData.Handler.Up;
                var upSub = _up.GetSubArray(index, jobUp.Length);
                jobUp.CopyTo(upSub);
                index += length;
            }
            
            UpdateTransforms transformJob = new()
            {
                Pos = _pos,
                Tan = _tan,
                Up = _up
            };
            
            _updateTransformHandle = transformJob.Schedule(_transformsAccess);
            MoveMarker.End();
        }

        private void RunLate()
        {
            _updateTransformHandle.Complete();
        }

        private void DisposeAndClear()
        {
            _targetCount = 0;
            foreach (var jobData in _mapping.Values)
            {
                jobData.Handler.ClearAndDispose();
            }
            _handles.Dispose();
            _transformsAccess.Dispose(); //TODO reuse instead of dispose
            _pos.Dispose();
            _tan.Dispose();
            _up.Dispose();
        }
    }
}