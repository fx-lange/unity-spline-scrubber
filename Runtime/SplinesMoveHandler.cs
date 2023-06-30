using System.Collections.Generic;
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
        [SerializeField] private int _capacity = 1000;

        static readonly ProfilerMarker ScheduleMarker = new(ProfilerCategory.Scripts, "SplinesMoveHandler.Schedule");
        static readonly ProfilerMarker EvaluateMarker = new(ProfilerCategory.Scripts, "SplinesMoveHandler.Evaluate");
        static readonly ProfilerMarker MoveMarker = new(ProfilerCategory.Scripts, "SplinesMoveHandler.Move");

        private class JobData
        {
            public NativeSpline Spline;
            public readonly SplineEvaluateHandler Handler = new();
        }

        private readonly Dictionary<SplineClipData, JobData> _mapping = new();
        private int _targetCount = 0;
        private TransformAccessArray _transformsAccess;
        private JobHandle _updateTransformHandle;

        private NativeArray<JobHandle> _evaluateHandles;
        private NativeArray<float3> _pos;
        private NativeArray<float3> _tan;
        private NativeArray<float3> _up;

        private ArrayListAccess<Transform> _transforms;

        private void OnEnable()
        {
            //TODO link (private run) into post director
        }

        private void OnDisable()
        {
            //TODO unlink
        }

        private void Awake()
        {
            _transforms = new ArrayListAccess<Transform>(_capacity);
            _transformsAccess = new TransformAccessArray(_capacity);
        }

        private void LateUpdate()
        {
            RunLate();
            DisposeAndClear();
        }

        public void UpdatePos(Transform target, float tPos, SplineClipData spline)
        {
            // using (ScheduleMarker.Auto())
            {
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
            }
        }

        public void Run()
        {
            EvaluateMarker.Begin();
            RunEvaluate();
            EvaluateMarker.End();

            MoveMarker.Begin();
            //collect results instead of running multiple transform jobs
            //for later blending support
            var prepareHandle = PrepareMove();
            RunMove(prepareHandle);
            MoveMarker.End();

            void RunEvaluate()
            {
                //run all evaluate jobs
                var jobCount = _mapping.Count;
                _evaluateHandles = new NativeArray<JobHandle>(jobCount, Allocator.TempJob);

                int jobIdx = 0;
                int cpyIndex = 0;
                foreach (var pair in _mapping)
                {
                    var jobData = pair.Value;
                    jobData.Handler.Prepare();

                    var count = jobData.Handler.Transforms.Count;

                    _evaluateHandles[jobIdx] = jobData.Handler.Run(jobData.Spline);
                    jobData.Handler.Transforms.CopyTo(_transforms.Array, cpyIndex);

                    cpyIndex += count;
                    jobIdx++;
                }

                _transformsAccess.SetTransforms(_transforms.Array);
            }

            JobHandle PrepareMove()
            {
                int count = _targetCount;
                _pos = new NativeArray<float3>(count, Allocator.TempJob);
                _tan = new NativeArray<float3>(count, Allocator.TempJob);
                _up = new NativeArray<float3>(count, Allocator.TempJob);

                int startIdx = 0;
                int index = 0;
                JobHandle sequenceHandle = JobHandle.CombineDependencies(_evaluateHandles);
                foreach (var pair in _mapping)
                {
                    var jobData = pair.Value;
                    var jobPos = jobData.Handler.Pos;
                    var jobTan = jobData.Handler.Tan;
                    var jobUp = jobData.Handler.Up;
                    var length = jobPos.Length;

                    CollectResultsJob collectJob = new()
                    {
                        PosIn = jobPos,
                        TanIn = jobTan,
                        UpIn = jobUp,
                        StartIdx = startIdx,
                        Length = length,
                        Pos = _pos,
                        Tan = _tan,
                        Up = _up
                    };
                    sequenceHandle = collectJob.Schedule(sequenceHandle);

                    startIdx += length;
                    index++;
                }

                return sequenceHandle;
            }

            void RunMove(JobHandle dependency)
            {
                UpdateTransforms transformJob = new()
                {
                    Pos = _pos,
                    Tan = _tan,
                    Up = _up
                };

                _updateTransformHandle = transformJob.Schedule(_transformsAccess, dependency);
            }
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
            
            _transforms.Clear();
            _evaluateHandles.Dispose();
            _pos.Dispose();
            _tan.Dispose();
            _up.Dispose();
        }

        private void OnDestroy()
        {
            _transformsAccess.Dispose();
        }
    }
}