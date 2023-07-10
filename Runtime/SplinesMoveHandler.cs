using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.Jobs;
using Transform = UnityEngine.Transform;

namespace SplineScrubber
{
    [ExecuteAlways]
    public class SplinesMoveHandler : MonoBehaviour
    {
        [SerializeField] private int _capacity = 1000;
        [SerializeField] private int _batchCount = 2;

        static readonly ProfilerMarker EvaluateMarker = new(ProfilerCategory.Scripts, "SplinesMoveHandler.Evaluate");
        static readonly ProfilerMarker MoveMarker = new(ProfilerCategory.Scripts, "SplinesMoveHandler.Move");

        private readonly List<SplineEvaluateHandler> _evaluateHandlers = new();

        private int _targetCount = 0;
        private TransformAccessArray _transformsAccess;

        private NativeArray<float3> _pos;
        private NativeArray<float3> _tan;
        private NativeArray<float3> _up;
        
        private NativeArray<JobHandle> _evaluateHandles;
        private JobHandle _prepareMoveHandle;
        private JobHandle _updateTransformHandle;
 
        private static SplinesMoveHandler _instance;
        private static bool _initialized;
        private bool _didRun;

        public int Capacity => _capacity;
        
        public static SplinesMoveHandler Instance
        {
            get
            {
                if (!_initialized)
                {
                    _instance = FindAnyObjectByType<SplinesMoveHandler>();
                    _initialized = _instance != null;
                }

                return _instance;
            }
        }

        private void OnEnable() //
        {
            _transformsAccess = new TransformAccessArray(_capacity);
        }

        private void Update()
        {
            
            RunMove(_prepareMoveHandle); //A
            FinishFrame();
        }

        private void LateUpdate()
        {
            RunEvaluate(); 
        }

        // private void OnPostRender()
        // {
            // RunMove(_prepareMoveHandle); //B
        // }
        
        public void Register(SplineEvaluateHandler handler)
        {
            _evaluateHandlers.Add(handler);
        }

        public void Unregister(SplineEvaluateHandler handler)
        {
            _evaluateHandlers.Remove(handler);
        }

        public int Schedule(Transform target)
        {
            _transformsAccess.Add(target);
            return _targetCount++;
        }

        private void RunEvaluate()
        {
            if (_targetCount == 0)
            {
                return;
            }
            
            EvaluateMarker.Begin();
            Run();
            EvaluateMarker.End();

            MoveMarker.Begin();
            //collect results instead of running multiple transform jobs
            //for later blending support
            PrepareMove();
            MoveMarker.End();

            _didRun = true;

            void Run()
            {
                //run all evaluate jobs
                var jobCount = _evaluateHandlers.Count;
                _evaluateHandles = new NativeArray<JobHandle>(jobCount, Allocator.TempJob);

                for (var idx = 0; idx < _evaluateHandlers.Count; idx++)
                {
                    var handler = _evaluateHandlers[idx];
                    handler.Prepare();
                    _evaluateHandles[idx] = handler.Run(_batchCount);
                }
            }

            void PrepareMove()
            {
                int count = _targetCount;
                _pos = new NativeArray<float3>(count, Allocator.TempJob);
                _tan = new NativeArray<float3>(count, Allocator.TempJob);
                _up = new NativeArray<float3>(count, Allocator.TempJob);
                
                _prepareMoveHandle = JobHandle.CombineDependencies(_evaluateHandles);
                foreach (var handler in _evaluateHandlers)
                {
                    CollectResultsJob collectJob = new()
                    {
                        Indices = handler.Indices,
                        PosIn = handler.Pos,
                        TanIn = handler.Tan,
                        UpIn = handler.Up,
                        Length = handler.Count,
                        Pos = _pos,
                        Tan = _tan,
                        Up = _up
                    };
                    _prepareMoveHandle = collectJob.Schedule(_prepareMoveHandle);
                }
            }
        }
        
        private void RunMove(JobHandle dependency)
        {
            if (!_didRun) return;
                
            UpdateTransforms transformJob = new()
            {
                Pos = _pos,
                Tan = _tan,
                Up = _up
            };
                
            _updateTransformHandle = transformJob.Schedule(_transformsAccess, dependency);
        }
        
        private void FinishFrame()
        {
            if (!_didRun) return;
            
            _updateTransformHandle.Complete();
            DisposeAndClear();
        }

        private void DisposeAndClear()
        {
            _didRun = false;
            _targetCount = 0;
            foreach (var evaluate in _evaluateHandlers)
            {
                evaluate.ClearAndDispose();
            }
            
            _transformsAccess.Dispose();
            _transformsAccess = new TransformAccessArray(_capacity);
            _evaluateHandles.Dispose();
            _pos.Dispose();
            _tan.Dispose();
            _up.Dispose();
        }
        
        private void OnDisable()
        {
            _prepareMoveHandle.Complete();
            FinishFrame();
            _transformsAccess.Dispose();
            _initialized = false;
        }
    }
}