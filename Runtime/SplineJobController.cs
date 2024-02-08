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
                if (!_init)
                {
                    Init();
                } 
                return _length;
            }
        }

        public SplinePath<Spline> SplinePath => _path;

        private SplineJobsScheduler _scheduler;
        private SplineEvaluateRunner _evaluateRunner;
        private float _length;
        private SplinePath<Spline> _path;
        private NativeSpline _nativeSpline;
        private bool _init;
        private bool _disposable;

        public void HandlePosUpdate(Transform target, float t)
        {
            if (!_evaluateRunner.ReadyForInput)
            {
                return; //can happen during drag clip
            }
            
            var idx = _scheduler.Schedule(target);
            _evaluateRunner.HandlePosUpdate(t,idx);
        }

        private void OnEnable()
        {
            if (_init)
            {
                return;
            }
            
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
            // EditorSplineUtility.AfterSplineWasModified += OnSplineModified;
            PrepareSplineData();
            _init = true;
        }

        private void OnDisable()
        {
            Spline.Changed -= OnSplineChanged;
            // EditorSplineUtility.AfterSplineWasModified -= OnSplineModified;
            Dispose();
            var moveInstance = SplineJobsScheduler.Instance;
            if (moveInstance != null)
            {
                moveInstance.Unregister(_evaluateRunner);
            }

            _init = false;
        }

        private void Update()
        {
            if (!_init)
            {
                return;
            }

            if (_container.transform.hasChanged)
            {
                PrepareSplineData();
            }
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
            var splineTransform = _container.transform;
            _length = _container.CalculateLength();
            _path = new SplinePath<Spline>(_container.Splines);

            Dispose();
            _nativeSpline = new NativeSpline(_path, splineTransform.localToWorldMatrix, Allocator.Persistent);
            _evaluateRunner.Spline = _nativeSpline;
            _disposable = true;
            
            splineTransform.hasChanged = false;
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