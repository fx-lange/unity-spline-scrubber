using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Jobs;
using UnityEngine.Splines;

namespace SplineScrubber
{
    public class SplinesMoveHandler : MonoBehaviour, ISplineJobHandler
    {
        private class JobData
        {
            public NativeSpline Spline;
            public readonly SplineEvaluateHandler Handler = new();
        }

        private Dictionary<SplineClipData, JobData> _mapping = new();
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
            if (!_mapping.ContainsKey(spline))
            {
                _mapping[spline] = new JobData()
                {
                    Spline = spline.NativeSpline
                };
            }

            var jobData = _mapping[spline];
            jobData.Handler.ScheduleEvaluate(target, tPos);
        }

        public void Run()
        {
            //run all jobs
            var jobCount = _mapping.Count;
            _handles = new NativeArray<JobHandle>(jobCount, Allocator.TempJob);
            var transforms = new List<Transform>();
            var jobDataList = _mapping.Values.ToList();
            for (var i = 0; i < jobCount; ++i)
            {
                var jobData = jobDataList[i];
                jobData.Handler.Prepare();
                _handles[i] = jobData.Handler.Run(jobData.Spline);
                transforms.AddRange(jobData.Handler.Transforms);
            }

            //TODO combine must be a job or we have to wait for complete here
            var combined = JobHandle.CombineDependencies(_handles);
            combined.Complete();
            
            int count = transforms.Count;
            _transformsAccess = new TransformAccessArray(transforms.ToArray());
            _pos = new NativeArray<float3>(count, Allocator.TempJob);
            _tan = new NativeArray<float3>(count, Allocator.TempJob);
            _up = new NativeArray<float3>(count, Allocator.TempJob);
            int index = 0;
            for (var i = 0; i < jobCount; ++i)
            {
                var jobData = jobDataList[i];
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
        }

        private void RunLate()
        {
            _updateTransformHandle.Complete();
        }

        private void DisposeAndClear()
        {
            foreach (var jobData in _mapping.Values)
            {
                jobData.Handler.ClearAndDispose();
            }
            _handles.Dispose();
            _transformsAccess.Dispose();
            _pos.Dispose();
            _tan.Dispose();
            _up.Dispose();
        }
    }
}