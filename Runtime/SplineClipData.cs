using System;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Splines;

namespace SplineScrubber
{
    /* TODO
     * edit vs playmode
     * cache on start or serializing
     */
    [ExecuteAlways]
    [DisallowMultipleComponent]
    public class SplineClipData : MonoBehaviour
    {
        [SerializeField] private SplineContainer _container;
        [SerializeField] private SplinesMoveHandler _splinesMoveHandler;
        
        public SplineEvaluateHandler JobHandler => _handler;
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

        private SplineEvaluateHandler _handler;
        private float _length;
        private SplinePath<Spline> _path;
        private NativeSpline _nativeSpline;
        private bool _init;
        private bool _disposable;

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

            if (_splinesMoveHandler == null)
            {
                _splinesMoveHandler = SplinesMoveHandler.Instance;
                if (_splinesMoveHandler == null)
                {
                    Debug.LogError("Missing SplineMoveHandler in the scene",this);
                    enabled = false;
                    return;
                }
            }

            _handler = new SplineEvaluateHandler(_splinesMoveHandler);

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
            var moveInstance = SplinesMoveHandler.Instance;
            if (moveInstance != null)
            {
                moveInstance.Unregister(_handler);
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
            _handler.Spline = _nativeSpline;
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