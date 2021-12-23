using System;
using System.Collections;
using System.Threading.Tasks;
using Path;
using UnityEngine;
using UnityEngine.Splines;

namespace Spline
{
    [ExecuteAlways]
    public class SplinePath : PathBase//MonoBehaviour, IPath
    {
        [SerializeField] private SplineContainer spline;

        public SplineContainer Spline => spline;
        public override float Length => length;

        [HideInInspector]
        public float length;

        private WaitForEndOfFrame _waitForEndOfFrame;
        private Coroutine _coroutine;
        
        private void OnEnable()
        {
            _waitForEndOfFrame = new WaitForEndOfFrame();

            if (spline == null)
            {
                enabled = false;
                return;
            }

            length = spline.CalculateLength();
            spline.Spline.changed += SplineOnChanged;
        }

        private void OnDisable()
        {
            spline.Spline.changed -= SplineOnChanged;
        }

        private bool _changeDetected = false;
        private async void SplineOnChanged()
        {
            if (_changeDetected)
            {
                return;
            }
            
            _changeDetected = true;
            await Task.Delay(2000);

            length = spline.CalculateLength();
            _changeDetected = false;
        }
    }
}