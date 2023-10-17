using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Serialization;
using UnityEngine.Timeline;

namespace SplineScrubber.Timeline
{
    [Serializable]
    public class SplineClipBehaviour : PlayableBehaviour
    {
        [NotKeyable] [Min(0.0001f)] [SerializeField]
        private float _speed = 1;

        [NotKeyable] [SerializeField] private bool _ignoreRotation;
        
        [NotKeyable] [Min(0)] [SerializeField] private float _accTime;
        [NotKeyable] [Min(0)] [SerializeField] private float _decTime;
        [SerializeField] private AnimationCurve _accCurve;
        [SerializeField] private AnimationCurve _decCurve;
        
        [SerializeField] private float _accDistance;
        [SerializeField] private float _decDistance;

        public SplineJobController SplineController { get; set; }
        public float Duration { private get; set; }
        public float Speed => _speed;
        public bool IgnoreRotation => _ignoreRotation;
        public float AccTime => _accTime;

        public float DecTime => _decTime;

        public void UpdateAccDecDistance()
        {
            _accDistance = AccTime * Speed / 2f;
            _decDistance = DecTime * Speed / 2f;
        }

        public double EvaluateDistance(double time)
        {
            if (Duration == 0) return 0;

            var innerTime = time % Duration;
            var loops = math.floor(time / Duration);
            return EvalInnerDistance(innerTime) + loops * SplineController.Length;

            double EvalInnerDistance(double t)
            {
                var mixInDur = AccTime;
                var mixOutDur = DecTime;
                var mixOutStart = Duration - mixOutDur;

                double mixInDistance = 0;
                if (mixInDur > 0)
                {
                    var mixInTime = math.min(t, mixInDur);
                    var mixInSpeed = mixInTime * Speed / mixInDur;
                    mixInDistance = mixInTime * mixInSpeed / 2f;
                }

                double mixOutDistance = 0;
                if (mixOutDur > 0)
                {
                    var mixOutTime = mixOutDur - math.clamp(t - mixOutStart, 0, mixOutDur);
                    var mixOutSpeed = mixOutTime * Speed / mixOutDur;
                    mixOutDistance = mixOutDur * Speed / 2f - mixOutTime * mixOutSpeed / 2f;
                }

                var centerDistance = math.clamp(t - mixInDur, 0, mixOutStart - mixInDur) * Speed;
                return mixInDistance + centerDistance + mixOutDistance;
            }
        }

        public override void OnPlayableCreate(Playable playable)
        {
        }
    }
}