using System;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

namespace SplineScrubber
{
    [Serializable]
    public class SplineEvaluateHandler : MonoBehaviour, ISplineEvaluate
    {
        [SerializeField] private int _capacity = 100;
        public int Count => Indices.Length;

        public NativeList<int> Indices { get; private set; }
        public NativeArray<float3> Pos { get; private set; }
        public NativeArray<float3> Tan { get; private set; }
        public NativeArray<float3> Up { get; private set; }
        public NativeSpline Spline { get; set; }

        private NativeList<float> _times;
        private SplinesMoveHandler _moveHandler;

        private void Awake()
        {
            Indices = new NativeList<int>(_capacity, Allocator.Persistent);
            _times = new NativeList<float>(_capacity, Allocator.Persistent);
        }

        private void Start()
        {
            _moveHandler = SplinesMoveHandler.Instance;
        }

        public void HandlePosUpdate(Transform target, float tPos)
        {
            var idx = _moveHandler.Schedule(target);
            _times.Add(tPos);
            Indices.Add(idx);
        }

        public void Prepare()
        {
            Pos = new NativeArray<float3>(Count, Allocator.TempJob);
            Tan = new NativeArray<float3>(Count, Allocator.TempJob);
            Up = new NativeArray<float3>(Count, Allocator.TempJob);
        }
        
        public JobHandle Run(int batchCount = 2)
        {
            SplineEvaluate evaluateJob = new()
            {
                Spline = Spline,
                ElapsedTimes = _times.AsArray(),
                Pos = Pos,
                Tan = Tan,
                Up = Up
            };
            return evaluateJob.Schedule(_times.Length, batchCount);
        }

        public void ClearAndDispose()
        {
            Indices.Clear();
            _times.Clear();
            Pos.Dispose();
            Tan.Dispose();
            Up.Dispose();
        }

        ~SplineEvaluateHandler()
        {
            _times.Dispose();
            Indices.Dispose();
        }
    }
}