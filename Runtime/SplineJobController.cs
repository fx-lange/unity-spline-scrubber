using Unity.Collections;
using UnityEngine;
using UnityEngine.Splines;

namespace SplineScrubber
{
    [ExecuteAlways]
    [DisallowMultipleComponent]
    public class SplineJobController : MonoBehaviour
    {
        [SerializeField] private SplineContainer _container;
        
        public float Length
        {
            get {
                if (!_lengthCached)
                {
                    CacheLength();
                } 
                return _length;
            }
        }

        public SplinePath<Spline> SplinePath => _path;

        private SplineJobsScheduler _scheduler;
        private SplineEvaluateRunner _evaluateRunner;
        private float _length;
        private bool _lengthCached;
        private SplinePath<Spline> _path;
        private NativeSpline _nativeSpline;
        private bool _init;
        private bool _disposable;

        public void HandlePosUpdate(Transform target, float t)
        {
            if (!enabled) return;
            if (!_evaluateRunner.ReadyForInput) return; //can happen during drag clip

            var idx = _scheduler.Schedule(target); 
            if (idx < 0) return;
            
            _evaluateRunner.HandlePosUpdate(t,idx);
        }

        private void OnEnable()
        {
            if (_init) return;
            Init();
        }

        private void Init()
        {
            if (_container == null)
            {
                if (!TryGetComponent(out _container))
                {
                    Debug.LogError("Missing SplineContainer!", this);
                    enabled = false;
                    return;
                }
            }

            if (_scheduler == null)
            {
                _scheduler = SplineJobsScheduler.Instance;
                if (_scheduler == null)
                {
                    Debug.LogWarning("Missing SplineJobsScheduler in the scene");
                    enabled = false;
                    return;
                }
            }

            _evaluateRunner = new SplineEvaluateRunner(_scheduler.Capacity);
            _scheduler.Register(_evaluateRunner);

            Spline.Changed += OnSplineChanged;
            PrepareSplineData();
            _init = true;
        }

        private void OnDisable()
        {
            Spline.Changed -= OnSplineChanged;
            
            if (_scheduler != null)
            {
                _scheduler.Unregister(_evaluateRunner);
            }
            Dispose();
            
            _init = false;
        }

        private void OnSplineChanged(Spline spline, int _, SplineModification __)
        {
            OnSplineModified(spline);
        }
        
        private void OnSplineModified(Spline spline)
        {
            if (_container.Spline != spline)
            {
                return;
            }
            
            PrepareSplineData();
        }

        private void PrepareSplineData()
        {
            CacheLength();
            
            _path = new SplinePath<Spline>(_container.Splines);

            Dispose();
            _nativeSpline = new NativeSpline(_path, Allocator.Persistent);
            _evaluateRunner.Spline = _nativeSpline;
            _evaluateRunner.SplineTransform = _container.transform;
            _disposable = true;
        }

        private void CacheLength()
        {
            if (_container == null)
            {
                _length = 0;
                return;
            }
            
            _length = _container.CalculateLength();
            _lengthCached = true;
        }

        private void Dispose()
        {
            if (!_disposable)
            {
                return;
            }
            
            _nativeSpline.Dispose();
            _disposable = false;
        }
    }
}