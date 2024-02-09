using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using Unity.Profiling;
using UnityEngine;

namespace SplineScrubber
{
    [ExecuteAlways]
    public class SplineJobsScheduler : MonoBehaviour
    {
        [SerializeField] private int _capacity = 1000;
        [SerializeField] private int _batchCount = 2;

        static readonly ProfilerMarker EvaluateMarker = new(ProfilerCategory.Scripts, "SplinesMoveHandler.Evaluate");
        static readonly ProfilerMarker MoveMarker = new(ProfilerCategory.Scripts, "SplinesMoveHandler.Move");

        private readonly List<SplineEvaluateRunner> _evaluateRunners = new();
        private TransformUpdateRunner _transformUpdateRunner = new();
        
        private NativeArray<JobHandle> _evaluateHandles;
        private JobHandle _prepareMoveHandle;
        private JobHandle _updateTransformHandle;
        
        private int _targetCount = 0;
        private bool _didRun;

        private static SplineJobsScheduler _instance;
        private static bool _initialized;

        public int Capacity => _capacity;

        public static SplineJobsScheduler Instance
        {
            get
            {
                if (!_initialized)
                {
                    _instance = FindAnyObjectByType<SplineJobsScheduler>();
                    _initialized = _instance != null;
                }

                return _instance;
            }
        }

        private void OnEnable()
        {
            if (Instance != this)
            {
                Debug.LogError("Scheduler already exists, abort!");
            }
            _transformUpdateRunner.Init(_capacity);
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

        public void Register(SplineEvaluateRunner runner)
        {
            _evaluateRunners.Add(runner);
        }

        public void Unregister(SplineEvaluateRunner runner)
        {
            _evaluateRunners.Remove(runner);
        }

        public int Schedule(Transform target)
        {
            _transformUpdateRunner.Schedule(target);
            return _targetCount++;
        }

        private void RunEvaluate()
        {
            if (_targetCount == 0)
            {
                return;
            }

            EvaluateMarker.Begin();
            Evaluate();
            EvaluateMarker.End();

            MoveMarker.Begin();
            //collect results instead of running multiple transform jobs
            //for later blending support
            PrepareMove();
            MoveMarker.End();

            _didRun = true;

            void Evaluate()
            {
                //run all evaluate jobs
                var jobCount = _evaluateRunners.Count;
                _evaluateHandles = new NativeArray<JobHandle>(jobCount, Allocator.TempJob);

                for (var idx = 0; idx < _evaluateRunners.Count; idx++)
                {
                    var evaluateRunner = _evaluateRunners[idx];
                    evaluateRunner.Prepare();
                    _evaluateHandles[idx] = evaluateRunner.Run(_batchCount);
                }
            }

            void PrepareMove()
            {
                int count = _targetCount;
                var evaluateHandle = JobHandle.CombineDependencies(_evaluateHandles);
                _prepareMoveHandle = _transformUpdateRunner.PrepareMove(count, _evaluateRunners, evaluateHandle);
            }
        }

        private void RunMove(JobHandle dependency)
        {
            if (!_didRun) return;
            _updateTransformHandle = _transformUpdateRunner.RunMove(dependency);
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
            foreach (var evaluate in _evaluateRunners)
            {
                evaluate.ClearAndDispose();
            }

            _transformUpdateRunner.Dispose();
            _evaluateHandles.Dispose();
            _transformUpdateRunner.Init(_capacity);
        }

        private void OnDisable()
        {
            _prepareMoveHandle.Complete();
            FinishFrame();
            _transformUpdateRunner.Dispose(); 
            _initialized = false;
        }
    }
}