using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Timeline;

namespace SplineScrubber.Timeline.Clips
{
    [Serializable]
    public class SplineTweenBehaviour : BaseSplineBehaviour
    {
        [SerializeField] [NotKeyable] [Range(0,1)] private float _from;
        [SerializeField] [NotKeyable] [Range(0,1)] private float _to = 1f;
        [SerializeField] private TweenType _tweenType;
        [SerializeField] private AnimationCurve _customCurve = AnimationCurve.Linear(0,0,1,1);
        
        public double Duration { private get; set; }
        
        public enum TweenType
        {
            Linear,
            EaseInOut,
            Custom
        }

        private AnimationCurve _linear = AnimationCurve.Linear(0, 0, 1, 1);
        private AnimationCurve _easeInOut = AnimationCurve.EaseInOut(0, 0, 1, 1);
        
        public override double EvaluateNormPos(double time)
        {
            if (_tweenType == TweenType.Custom && !IsCustomCurveNormalised ())
            {
                Debug.LogError("Custom Curve is not normalised.  Curve must start at 0,0 and end at 1,1.");
                return 0f;
            }
            
            var timeNormalized = (float)(time / Duration);
            timeNormalized = _tweenType switch
            {
                TweenType.Linear => _linear.Evaluate(timeNormalized),
                TweenType.EaseInOut => _easeInOut.Evaluate(timeNormalized),
                TweenType.Custom => _customCurve.Evaluate(timeNormalized),
                _ => throw new ArgumentOutOfRangeException()
            };

            var pos = math.lerp(_from, _to, timeNormalized);
            return pos;
        }
        
        private bool IsCustomCurveNormalised ()
        {
            if (!Mathf.Approximately (_customCurve[0].time, 0f))
                return false;
        
            if (!Mathf.Approximately (_customCurve[0].value, 0f))
                return false;
        
            if (!Mathf.Approximately (_customCurve[_customCurve.length - 1].time, 1f))
                return false;
        
            return Mathf.Approximately (_customCurve[_customCurve.length - 1].value, 1f);
        }
    }
}