using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.Jobs;
using UnityEngine.LowLevel;
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

        private PlayerLoopSystem _directorEvaluateLoop;

        private static SplinesMoveHandler _instance;
        private static bool _awake;
        private bool _didRun;
        private int _runFrame = -1;
        private bool _attached;

        public static SplinesMoveHandler Instance
        {
            get
            {
                if (!_awake)
                {
                    Debug.LogWarning("Not awake yet");
                    return null;
                }

                return _instance;
            }
        }

        private void Awake()
        {
            _instance = this;
            _awake = true;
            _transformsAccess = new TransformAccessArray(_capacity);
            // AttachRun(); //A
        }

        private void Update()
        {
            FinishFrame(); //B
        }

        private void LateUpdate()
        {
            // FinishFrame(); //A
            Run(); //B
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

                jobData.Handler.ScheduleEvaluate(_targetCount++, tPos);
                _transformsAccess.Add(target);
            }
        }

        public void Run()
        {
            if (!enabled)
            {
                return;
            }
            
            _runFrame = Time.frameCount;
            if (_targetCount == 0)
            {
                return;
            }
            
            EvaluateMarker.Begin();
            RunEvaluate();
            EvaluateMarker.End();

            MoveMarker.Begin();
            //collect results instead of running multiple transform jobs
            //for later blending support
            var prepareHandle = PrepareMove();
            RunMove(prepareHandle);
            MoveMarker.End();

            _didRun = true;

            void RunEvaluate()
            {
                //run all evaluate jobs
                var jobCount = _mapping.Count;
                _evaluateHandles = new NativeArray<JobHandle>(jobCount, Allocator.TempJob);

                int jobIdx = 0;
                foreach (var pair in _mapping)
                {
                    var jobData = pair.Value;
                    jobData.Handler.Prepare();

                    var count = jobData.Handler.Count;

                    _evaluateHandles[jobIdx] = jobData.Handler.Run(jobData.Spline);
                    jobIdx++;
                }
            }

            JobHandle PrepareMove()
            {
                int count = _targetCount;
                _pos = new NativeArray<float3>(count, Allocator.TempJob);
                _tan = new NativeArray<float3>(count, Allocator.TempJob);
                _up = new NativeArray<float3>(count, Allocator.TempJob);

                JobHandle sequenceHandle = JobHandle.CombineDependencies(_evaluateHandles);
                foreach (var pair in _mapping)
                {
                    var handler = pair.Value.Handler;

                    CollectResultsJob collectJob = new()
                    {
                        Indices = handler.Indices,
                        PosIn = handler.Pos,
                        TanIn = handler.Tan,
                        UpIn = handler.Up,
                        Pos = _pos,
                        Tan = _tan,
                        Up = _up
                    };
                    sequenceHandle = collectJob.Schedule(handler.Count, 2, sequenceHandle);

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
        
        private void FinishFrame()
        {
            // Debug.Log($"{Time.frameCount} LateUpdate");
            if (_didRun == false)
            {
                return;
            }

            _updateTransformHandle.Complete();
            DisposeAndClear();
        }

        private void DisposeAndClear()
        {
            _didRun = false;
            _targetCount = 0;
            foreach (var jobData in _mapping.Values)
            {
                jobData.Handler.ClearAndDispose();
            }
            
            _transformsAccess.Dispose();
            _transformsAccess = new TransformAccessArray(_capacity);
            _evaluateHandles.Dispose();
            _pos.Dispose();
            _tan.Dispose();
            _up.Dispose();
        }

        private void OnDestroy()
        {
            FinishFrame();
            _transformsAccess.Dispose();
            DetachRun();
        }
        
        private void AttachRun()
        {
            var playerLoop = PlayerLoop.GetCurrentPlayerLoop();
            var preLateUpdateLoop = playerLoop.subSystemList[6];
            var directorEvaluateLoop = preLateUpdateLoop.subSystemList[4];
            preLateUpdateLoop.updateDelegate += PreLateUpdateDelegate; //works
            directorEvaluateLoop.updateDelegate += DirectorEvaluateDelegate; //ignored
            preLateUpdateLoop.subSystemList[4] = directorEvaluateLoop;
            playerLoop.subSystemList[6] = preLateUpdateLoop;
            PlayerLoop.SetPlayerLoop(playerLoop);
            _attached = true;
        }

        private void DetachRun()
        {
            if (!_attached)
            {
                return;
            }

            var playerLoop = PlayerLoop.GetCurrentPlayerLoop();
            var preLateUpdateLoop = playerLoop.subSystemList[6];
            var directorEvaluateLoop = preLateUpdateLoop.subSystemList[4];
            preLateUpdateLoop.updateDelegate -= PreLateUpdateDelegate; //works
            directorEvaluateLoop.updateDelegate -= DirectorEvaluateDelegate; //ignored
            preLateUpdateLoop.subSystemList[4] = directorEvaluateLoop;
            playerLoop.subSystemList[6] = preLateUpdateLoop;
            PlayerLoop.SetPlayerLoop(playerLoop);
            _attached = false;
        }
        
        private void PreLateUpdateDelegate()
        {
            Run();
        }
        
        private void DirectorEvaluateDelegate()
        {
            throw new NotImplementedException();
        }
    }
}