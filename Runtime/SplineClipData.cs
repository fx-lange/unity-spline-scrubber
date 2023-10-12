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
    public class SplineClipData : MonoBehaviour
    {
        [SerializeField] private SplineContainer _container;

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
                Debug.LogError("Missing SplineContainer!");
                enabled = false;
                return;
            }

            _handler = new SplineEvaluateHandler();

            Spline.Changed += OnSplineChanged;
            // EditorSplineUtility.AfterSplineWasModified += OnSplineModified;
            CacheData();
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
            
            CacheData();
        }

        private void CacheData()
        {
            _length = _container.CalculateLength();
            _path = new SplinePath<Spline>(_container.Splines);
            
            _nativeSpline.Dispose();
            _nativeSpline = new NativeSpline(_path, _container.transform.localToWorldMatrix, Allocator.Persistent);
            _handler.Spline = _nativeSpline;
        }

        private void Dispose()
        {
            _nativeSpline.Dispose();
        }
    }
}