using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace SplineScrubber.Timeline
{
    [Serializable]
    public class SplineClipBehaviour : PlayableBehaviour
    {
        [SerializeField] private SplineClipData _splineData;

        [NotKeyable] [Min(0.0001f)] [SerializeField]
        private float _speed = 1;

        [NotKeyable] [Min(0)] [SerializeField] private float _accTime;
        [NotKeyable] [Min(0)] [SerializeField] private float _decTime;

        public SplineClipData SplineData { get; set; }
        public float Duration { private get; set; }
        public float Speed => _speed;
        public float AccTime => _accTime;
        public float DecTime => _decTime;

        public double EvaluateDistance(double time)
        {
            if (Duration == 0) return 0;

            var innerTime = time % Duration;
            var loops = math.floor(time / Duration);
            return EvalInnerDistance(innerTime) + loops * SplineData.Length;

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